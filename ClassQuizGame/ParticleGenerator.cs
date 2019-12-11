using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClassQuizGame
{
    public class ParticleGenerator
    {
        public Boolean active;

        private Random random;
        private FireworkBase[] fireworks;
        private const int FIREWORKS_COUNT = 10;

        internal const float GRAVITY = 100.0f;

        private long lastMilli;
        private const float TIME_RESCALE = 0.001f;

        private static float yScale = 1.0f;
        public static float YScale
        {
            set
            {
                yScale = value;
                FireworkBase.yScale = yScale;
            }
        }
        private static float xScale = 1.0f;
        public static float XScale
        {
            set
            {
                xScale = value;
                FireworkBase.xScale = xScale;
            }
        }

        public ParticleGenerator(Canvas canvas)
        {
            active = false;
            random = new Random(DateTime.Now.Millisecond);
            fireworks = new FireworkBase[FIREWORKS_COUNT];
            for (int i = 0; i < FIREWORKS_COUNT; i++)
            {
                fireworks[i] = new FireworkBase(canvas);
            }
            lastMilli = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void update()
        {
            if (active)
            {
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                float deltaTime = now - lastMilli;
                lastMilli = now;
                deltaTime *= TIME_RESCALE;
                if (deltaTime < 0) deltaTime = 0;
                byte[] rgb = new byte[3];
                for (int i = 0; i < FIREWORKS_COUNT; i++)
                {
                    if (!fireworks[i].alive)
                    {
                        int x = random.Next(
                            (int)(FireworkBase.MIN_X * FireworkBase.xScale), 
                            (int)(FireworkBase.MAX_X * FireworkBase.xScale));
                        int sound = random.Next(1, 6);
                        random.NextBytes(rgb);
                        float angle = FireworkBase.MIN_ANGLE + ((float)(random.NextDouble() * FireworkBase.ANGLE));
                        float speed = FireworkBase.MIN_SPEED + ((float)(random.NextDouble() * FireworkBase.SPEED));
                        float delay = ((float)(random.NextDouble() * FireworkBase.DELAY));
                        fireworks[i].spawn(x, rgb[0], rgb[1], rgb[2], angle, speed, delay, sound);
                    }
                    else { }
                    fireworks[i].update(deltaTime);
                }
            }
            else { }
        }

        public static void setScale(float x, float y)
        {
            XScale = x;
            YScale = y;
        }

        public void rescale()
        {
            for (int i = 0; i < FIREWORKS_COUNT; i++)
            {
                fireworks[i].rescale();
            }
        }
    }

    public class FireworkBase
    {
        public static int MIN_X = 100;
        public static int MAX_X = 700;
        public static float xScale = 1.0f;
        private const int Y = 675;
        public static float yScale = 1.0f;
        private const int LEN = 7;
        private const int THICKNESS = 3;
        public Line firework;
        private SolidColorBrush brush;

        public static float MIN_ANGLE = ((float)(3.0f * Math.PI / 8.0f));
        public static float ANGLE = ((float)(Math.PI / 4.0f));

        private float timer;
        private float speedX;
        private float speedY;

        private float launchSpeed;
        private const float LAUNCH_MODIFIER = 1.25f;
        private const float BOOSTER_CUTOFF = 0.15f;

        private float deltaX;
        private float deltaY;
        public const float MIN_SPEED = 250.0f;
        public const float SPEED = 50.0f;
        public const float DELAY = 10.0f;
        private const float EXPLODE_DELAY = 0.25f;

        private byte red;
        private byte green;
        private byte blue;

        public Boolean alive;

        private MediaPlayer launch;
        private MediaPlayer explosion;

        private FireworkTrail[] trail;
        private const int TRAIL_COUNT = 20;
        private const float TRAIL_SPACING = 0.05f;
        private float trailSpawn;
        private int trailIndex;

        private Boolean launched;

        private Boolean exploded;
        private float explodeTimer;
        private const float EXPLODE_SPEED = 10.0f;

        private FireworkSpark[] sparks;
        private const int SPARK_LAYERS = 7;
        private const int INNER_SPARK_COUNT = 3;
        private int SPARK_COUNT;

        public FireworkBase(Canvas canvas)
        {
            firework = new Line();
            brush = new SolidColorBrush();
            firework.Stroke = brush;
            trail = new FireworkTrail[TRAIL_COUNT];
            for (int i = 0; i < TRAIL_COUNT; i++)
            {
                trail[i] = new FireworkTrail(canvas);
            }
            SPARK_COUNT = 0;
            for (int i = 0; i < SPARK_LAYERS; i++)
            {
                SPARK_COUNT += INNER_SPARK_COUNT * i + 1;
            }
            sparks = new FireworkSpark[SPARK_COUNT];
            for (int i = 0; i < SPARK_COUNT; i++)
            {
                sparks[i] = new FireworkSpark(canvas);
            }
            canvas.Children.Add(firework);
            explosion = new MediaPlayer();
            launch = new MediaPlayer();
            explosion.ScrubbingEnabled = false;
            exploded = false;
            alive = false;
        }

        public void spawn(int x, byte r, byte g, byte b, float angle, float speed, float delay, int sound)
        {
            red = r;
            green = g;
            blue = b;
            brush.Color = (Color.FromArgb(255, red, green, blue));
            firework.StrokeThickness = THICKNESS;
            firework.X1 = x;
            firework.Y1 = Y * yScale;
            deltaX = (float)((Math.Cos(angle)));
            firework.X2 = (deltaX * LEN) + firework.X1;
            deltaY = -(float)((Math.Sin(angle))) * LAUNCH_MODIFIER; // Moving up
            firework.Y2 = (deltaY * LEN) + (firework.Y1);
            speedX = deltaX * speed * xScale;
            speedY = deltaY * speed * yScale;
            launchSpeed = speedY;
            timer = delay;
            trailSpawn = TRAIL_SPACING;
            trailIndex = 0;
            exploded = false;
            explodeTimer = EXPLODE_DELAY;
            explosion.Open(new Uri(@"assets/firework_"+sound+".mp3", UriKind.Relative));
            launch.Open(new Uri(@"assets/firework_" + "launch" + ".mp3", UriKind.Relative));
            launched = false;
            //this.angle = (float.IsNaN(angle)) ? 0 : angle;
            alive = true;
        }

        public void update(float deltaTime)
        {
            if (alive)
            {
                if (!exploded)
                {
                    timer -= deltaTime;
                    if (timer < 0.0f)
                    {
                        if (!launched)
                        {
                            if (!Settings.Mute)
                            {
                                launch.Play();
                            }
                            else { }
                            launched = true;
                        }
                        else { }
                        updatePosition(deltaTime);
                        if (speedY >= 0.0f)
                        {
                            explodeTimer -= deltaTime;
                            if (explodeTimer < 0.0f)
                            {
                                explode();
                            }
                            else { }
                        }
                        else { }
                    }
                    else { }
                }
                else
                {
                    updateExplosions(deltaTime);
                }
                updateTrail(deltaTime);
            }
            else { }
        }

        private void updatePosition(float deltaTime)
        {
            //float angle = (float)Math.Asin(Math.Abs(speedY / (SPEED + MIN_SPEED)));
            //angle = (float.IsNaN(angle)) ? 0 : angle;
            firework.X1 += speedX * deltaTime;
            /*if (speedX > 0.0f)
            {
                firework.X2 = firework.X1 + ((float)((Math.Cos(angle))) * LEN);
            }
            else
            {
                firework.X2 = firework.X1 - ((float)((Math.Cos(angle))) * LEN);
            }*/
            firework.X2 += speedX * deltaTime;
            firework.Y1 += speedY * deltaTime;
            //firework.Y2 = firework.Y1 - Math.Abs(((float)((Math.Sin(angle))) * LEN));
            firework.Y2 += speedY * deltaTime;
            speedY += ParticleGenerator.GRAVITY * deltaTime * yScale;
        }

        private void updateTrail(float deltaTime)
        {
            for (int i = 0; i < TRAIL_COUNT; i++)
            {
                trail[i].update(deltaTime);
            }
            if (!exploded && speedY < (launchSpeed * BOOSTER_CUTOFF))
            {
                trailSpawn -= deltaTime;
                if (trailSpawn < 0)
                {
                    trail[trailIndex].spawn((int)firework.X1, (int)firework.Y1, speedX, speedY);
                    trailIndex = (++trailIndex) % TRAIL_COUNT;
                    trailSpawn = TRAIL_SPACING;
                }
                else { }
            }
            else { }
        }

        private void updateExplosions(float deltaTime)
        {
            alive = false;
            for (int i = 0; i < SPARK_COUNT; i++)
            {
                sparks[i].update(deltaTime);
                if (sparks[i].alive)
                {
                    alive = true;
                }
                else { }
            }
        }

        private void explode()
        {
            firework.StrokeThickness = 0;
            int index = 0;
            for (int i = 0; i < SPARK_LAYERS; i++)
            {
                int count = i * INNER_SPARK_COUNT + 1;
                for (int j = 0; j < count; j++)
                {
                    float angle = j * (((float)(2.0f * Math.PI)) / ((float)count));
                    float vx = ((float)Math.Cos(angle)) * (EXPLODE_SPEED * (i));
                    float vy = ((float)Math.Sin(angle)) * (EXPLODE_SPEED * (i));
                    sparks[index++].spawn((int)firework.X1, (int)firework.Y1, red, green, blue, vx, vy);
                }
            }
            explosion.Stop();
            if (!Settings.Mute)
            {
                explosion.Play();
            }
            else { }
            exploded = true;
        }

        public void rescale()
        {
            if (timer > 0.0f)
            {
                firework.X1 *= xScale;
                firework.Y1 *= yScale;
                firework.X2 *= xScale;
                firework.Y2 *= yScale;
                speedX *= xScale;
                speedY *= yScale;
                launchSpeed *= yScale;
            }
            else { }
        }
    }

    public class FireworkTrail
    {
        private const float BACK = 0.1f;
        private const int LENGTH = 2;
        private const int THICKNESS = 2;
        public Line trail;
        internal SolidColorBrush brush;
        private Color startColor = Color.FromArgb(255, 255, 165, 45);
        private float A;

        private const float DECAY = 500.0f;

        public Boolean alive;

        public FireworkTrail(Canvas canvas)
        {
            trail = new Line();
            brush = new SolidColorBrush();
            trail.Stroke = brush;
            canvas.Children.Add(trail);
            alive = false;
        }

        public void spawn(int x, int y, float vx, float vy)
        {
            brush.Color = startColor;
            float angle = ((float)(Math.Atan2(vy, vx)));
            trail.X1 = x + (BACK * Math.Cos(angle));
            trail.X2 = x + ((BACK+LENGTH) * Math.Cos(angle));
            trail.Y1 = y + (BACK * Math.Sin(angle));
            trail.Y2 = y + ((BACK+LENGTH) * Math.Sin(angle));
            trail.StrokeThickness = THICKNESS;
            A = 255.0f;
            alive = true;
        }

        public void update(float deltaTime)
        {
            if (alive)
            {
                Color currentColor = brush.Color;
                A -= DECAY * deltaTime;
                if (A < 0.0f)
                {
                    currentColor.A = 0;
                }
                else
                {
                    currentColor.A = ((byte)A);
                }
                brush.Color = currentColor;
                if (currentColor.A == 0)
                {
                    trail.StrokeThickness = 0;
                    alive = false;
                }
                else { }
            }
            else { }
        }
    }

    public class FireworkSpark
    {
        private const float GRAVITY_ADJ = 0.25f; 

        private const float BACK = 0.1f;
        private const int LENGTH = 2;
        private const int THICKNESS = 2;
        private Line spark; 
        private SolidColorBrush brush;
        private float A;

        private float deltaX;
        private float deltaY;

        private FireworkTrail[] sparkTrail;
        private int SPARK_TRAIL_COUNT = 5;
        private const float SPARK_TRAIL_SPACING = 0.05f;
        private float sparkTrailSpawn;
        private int sparkTrailIndex;

        public Boolean vanished;
        public Boolean alive;

        private float decayTimer;
        private float DECAY = 5.0f;
        private float DECAY_RATE = 50.0f; // 150.0f;
        private const float FLICKER_START = 4.0f;
        private const float FLICKER_STOP = 0.20f;
        private const float FLICKER = 1.0f;
        private const float FLICKER_RATE = 200.0f;
        private float flickerTimer;
        private Boolean flickerIn;

        public FireworkSpark(Canvas canvas)
        {
            spark = new Line();
            brush = new SolidColorBrush();
            spark.Stroke = brush;
            canvas.Children.Add(spark);
            sparkTrail = new FireworkTrail[SPARK_TRAIL_COUNT];
            for (int i = 0; i < SPARK_TRAIL_COUNT; i++)
            {
                sparkTrail[i] = new FireworkTrail(canvas);
            }
            flickerIn = false;
            alive = false;
        }

        public void spawn(int x, int y, byte r, byte g, byte b, float vx, float vy)
        {
            deltaX = vx;
            deltaY = vy;
            float angle = ((float)(Math.Atan2(deltaY, deltaX)));
            brush.Color = Color.FromArgb(255, r, g, b);
            spark.X1 = x + (BACK * Math.Cos(angle));
            spark.X2 = x + ((BACK + LENGTH) * Math.Cos(angle));
            spark.Y1 = y + (BACK * Math.Sin(angle));
            spark.Y2 = y + ((BACK + LENGTH) * Math.Sin(angle));
            spark.StrokeThickness = THICKNESS;
            A = 255.0f;
            for (int i = 0; i < SPARK_TRAIL_COUNT; i++)
            {
                sparkTrail[i].brush.Color = brush.Color;
            }
            decayTimer = DECAY;
            sparkTrailIndex = 0;
            flickerTimer = FLICKER_RATE;
            flickerIn = false;
            vanished = false;
            alive = true;
        }

        public void update(float deltaTime)
        {
            float angle = ((float)(Math.Atan2(deltaY, deltaX)));
            angle = (float.IsNaN(angle)) ? 0.0f : angle;
            spark.X1 += deltaX * deltaTime;
            if (deltaX > 0.0f)
            {
                spark.X2 = spark.X1 + ((float)((Math.Cos(angle))) * LENGTH);
            }
            else
            {
                spark.X2 = spark.X1 - ((float)((Math.Cos(angle))) * LENGTH);
            }
            spark.Y1 += deltaY * deltaTime;
            spark.Y2 = spark.Y1 - Math.Abs(((float)((Math.Sin(angle))) * LENGTH));
            deltaY += ParticleGenerator.GRAVITY * GRAVITY_ADJ * deltaTime;

            Color currentColor = brush.Color;
            decayTimer -= deltaTime;
            if (decayTimer >= FLICKER_START)
            {
                A -= DECAY_RATE * deltaTime;
            }
            else if (decayTimer >= FLICKER_STOP)
            {
                if (flickerIn)
                {
                    A += FLICKER_RATE * deltaTime;
                    flickerTimer -= deltaTime;
                    if (flickerTimer <= 0.0f)
                    {
                        flickerTimer = FLICKER;
                        flickerIn = false;
                    }
                    else { }
                }
                else
                {
                    A -= FLICKER_RATE * deltaTime;
                    flickerTimer -= deltaTime;
                    if (flickerTimer <= 0.0f)
                    {
                        flickerTimer = FLICKER;
                        flickerIn = true;
                    }
                    else { }
                }
            }
            else // if (decayTimer < FLICKER_STOP)
            {
                A -= DECAY_RATE * deltaTime;
            }

            if (A <= 0.0f)
            {
                currentColor.A = 0;
            }
            else if (A >= 255.0f)
            {
                currentColor.A = 255;
            }
            else
            {
                currentColor.A = ((byte)A);
            }
            brush.Color = currentColor;
            if (currentColor.A == 0)
            {
                spark.StrokeThickness = 0;
                vanished = true;
            }
            else { }
            for (int i = 0; i < SPARK_TRAIL_COUNT; i++)
            {
                Color trailColor = sparkTrail[i].brush.Color;
                trailColor.A = currentColor.A;
                sparkTrail[i].brush.Color = trailColor;
            }

            updateTrail(deltaTime);
        }

        private void updateTrail(float deltaTime)
        {
            for (int i = 0; i < SPARK_TRAIL_COUNT; i++)
            {
                sparkTrail[i].update(deltaTime);
            }
            if (!vanished)
            {
                sparkTrailSpawn -= deltaTime;
                if (sparkTrailSpawn < 0)
                {
                    sparkTrail[sparkTrailIndex].spawn((int)spark.X1, (int)spark.Y1, deltaX, deltaY);
                    sparkTrail[sparkTrailIndex].brush.Color = brush.Color;
                    sparkTrailIndex = (++sparkTrailIndex) % SPARK_TRAIL_COUNT;
                    sparkTrailSpawn = SPARK_TRAIL_SPACING;
                }
                else { }
            }
            else
            {
                alive = false;
                for (int i = 0; i < SPARK_TRAIL_COUNT; i++)
                {
                    if (sparkTrail[i].alive)
                    {
                        alive = true;
                        break;
                    }
                    else { }
                }
            }
        }
    }
}
