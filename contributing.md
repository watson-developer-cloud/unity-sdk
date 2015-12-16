# Questions

If you are having problems using the APIs or have a question about the IBM
Watson Services, please ask a question on
[dW Answers](https://developer.ibm.com/answers/questions/ask/?topics=watson)
or [Stack Overflow](http://stackoverflow.com/questions/ask?tags=ibm-watson).

# Coding Standard

* Use spaces instead of tab characters, a tab should be 4 spaces.
* Class names should be camel case (e.g. AvatarWidget), with no underscores "_".
* Class member fields should be camel case and begin with "m_" (e.g. m_MemberVariable), no other underscores allowed in the variable name.
* Static class member fields should camel case and begin with "sm_", no other underscores allowed in the variable name.
* Structures should follow the same naming standards as classes.
* No K&R coding style. Braces {} should be on their own line, aligned with the parent statement.
* No public variables, always use public properties unless there is no other workaround.
* Properties should be camel case, no underscores. (e.g. public bool IsReady { get; set; })
* Constants should be all upper case (e.g. static readonly string CONFIG_FILE = "/Config.json"). This includes enumerations.
* All public functions and types of all classes should be fully documented using the XML comment style.
* Local variables should begin with lower-case character, then camel case after the first character. (e.g. var widgetConnector = new WidgetConnector())
* Use protected on variables & functions only if you plan to inherit from the class or there is a good chance we will need to be polymorphic. 
* Use region to separate parts of a class based on functionality.
 
# Issues

If you encounter an issue with the Unity SDK library, you are welcome to submit
a [bug report](https://github.com/watson-developer-cloud/unity-sdk/issues).
Before that, please search for similar issues. It's possible somebody has
already encountered this issue.

# Pull Requests

If you want to contribute to the repository, follow these steps:

1. Fork the repo.
1. Develop and test your code changes Make sure you work in the `develop` branch. PLEASE don't do your work in `master`.
1. Add a unit test for any new classes you add. Only refactoring and documentation changes require no new tests.
1. Run the Watson->Run All UnitTest inside of Unity, make sure all tests work.
1. Commit your changes.
1. Push to your fork and submit a pull request.

# Developer's Certificate of Origin 1.1

By making a contribution to this project, I certify that:

(a) The contribution was created in whole or in part by me and I
   have the right to submit it under the open source license
   indicated in the file; or

(b) The contribution is based upon previous work that, to the best
   of my knowledge, is covered under an appropriate open source
   license and I have the right under that license to submit that
   work with modifications, whether created in whole or in part
   by me, under the same open source license (unless I am
   permitted to submit under a different license), as indicated
   in the file; or

(c) The contribution was provided directly to me by some other
   person who certified (a), (b) or (c) and I have not modified
   it.

(d) I understand and agree that this project and the contribution
   are public and that a record of the contribution (including all
   personal information I submit with it, including my sign-off) is
   maintained indefinitely and may be redistributed consistent with
   this project or the open source license(s) involved.
