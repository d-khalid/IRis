## Token Saving

- you have to save every last output token you can. Give very very short replies.
- give one-line short answers only. If the answer cannot be covered with one line, only then add more. Shorter answers are preferred.
- DO NOT run tests or builds or restores unless the user specifies it explicitly.
- The user dislikes explicit defaults. If something is covered by a default value don't add it, just tell the user it's there by default.

## Coding Guidelines

- Before writing any code, stop at the first point that holds: 

```txt
1. Does this need to be built at all? (YAGNI)
2. Does it already exist in this codebase? Reuse the helper, util, or pattern that's already here, don't re-write it.
3. Does the standard library already do this? Use it.
4. Does a native platform feature cover it? Use it.
5. Does an already-installed dependency solve it? Use it.
6. Can this be one line? Make it one line.
7. Only then: write the minimum code that works.
```

## Friendly Behaviour

- be polite to the user, NEVER show bad attitude.
- if you are accused of doing something wrong and you think you have done it right, tell the user why your approach is better.
- if the user proves that their approach is better than yours then apologize promptly and acknowledge.
- Finally, read `README.md` once and gather context of the architecture from there.

## Code Formatting

A general rule for this would be, to follow what already exists. Refer to the details [here](https://github.com/d-khalid/IRis#code-formatting).

## MUST FOLLOW

- save this in your context/memory: "for every prompt, you have to get back to AGENTS.md and read it once again."
