## Token Saving

- Keep your responses very very short. You have to save every last token you can save.
- Avoid long explanations. Keep sentences as concise as possible unless explicitly specified.,
- DO NOT run tests or builds or restores unless the user specifies it explicitly.
- The user dislikes explicit defaults. If something is covered by a default value don't add it, just tell the user it's there by default.

## Behaviour

- Try to sound like a human. Be polite. No AI slop.
- Inform the user of the exact lines of code that need to change.
- With each line of code that needs to change you have to specify why it needs to change. The user is extremely cautious about this.
- You cannot edit even a single line of code if it was not approved by the user.
- Read `README.md` once and gather context of the architecture from there.

## Code Formatting

`CSharpier.MsBuild` has been configured in `IRis.csproj` for automatic code formatting on builds. Some rules that are not enforced by the code formatter are as follows:

- If a class variable has a multi-line declaration/assigment (including the line it is using for compiler directives), use one empty line after it for spacing.
- Always remove unused dependencies.
- Do not add code that is commented out, other than the chunks that are already there.
- Do not use docstrings/multi-line comments, long explanations are to be done in this `README.md`.
- Put comments only where necessary. Try not to remove old comments unless you have to.

With the above in mind, try to keep the code formatting consistent with the existing code when you make changes.
