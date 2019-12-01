ClassQuizGame

A program for running quizzes. Requires Windows.
Supports XBox controllers or keyboard for single-player if no controllers are available.

The proctor can press space to advance through screens, though various options affect this flow (as seen in the menus).

You can click on the team names and scores to rename a team or adjust a score, respectively, at any time.

Quizzes use this format:
Begin with a <Quiz> tag which optionally takes the "folder" attribute to specify where images can be found. Note that this is relative to the binary, not the quiz.
Within the <Quiz> tag, specify a <Title> tag with the title of the quiz as its inner text. 
Then while still within the <Quiz> tag, add <Question> elements.
The <Question> tag should specify the attributes "choices" ("2" or "4") and "answer" ("L" or "R" for 2 choices; "A", "B", "X", or "Y" for 4 choices).
The <Question> tag can additionally specify an "image" tag and image source for an image, a "points" tag and integer value for how much this particular question is worth when correct (if not the default), or similarly a "penalty" tag and integer value for an incorrect answer, the tag "must_answer" with "true" or "false" if the question requires all players to answer before finishing, and/or a "timer" tag and integer value to add a timer to the question of the specified number of seconds.
The question is given as the inner text of the tag. Within the <Question> tag, the answer tags <L> and <R> for 2 choice questions or <A>, <B>, <X>, and <Y> for 4 choice questions must be specified.
The answer tags can have a "image" attribute with an image source specified to include an image, and the text of the answer is given as the inner text.

Note that the file extension does not matter.

I have also included a quiz on Konglish (and hotdogs), which demonstrates all the tags and attributes.

There are a few bugs, especially regarding the (re)sizing of some elements and sounds. 
Also there is no consistent naming convention and no comments.
I may fix everyone one day, but it is unlikely.

Have fun!