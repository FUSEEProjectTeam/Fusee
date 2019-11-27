Basicaly git-flow is just a formalized workflow for git. The git-flow toolset is just a layer on top to make staying in the bounds of the workflow easier. More infos on git-flow at the end of this page.

You can set up the git-flow toolset for your clone of FUSEE by simply executing `git flow init` in the repository. We are using the default settings so the Q&A dialog following the command should look like this:

```
Which branch should be used for bringing forth production releases?
Branch name for production releases: [master]

Which branch should be used for integration of the "next release"?
Branch name for "next release" development: [develop]

How to name your supporting branch prefixes?
Feature branches? [feature/]
Bugfix branches? [bugfix/]
Release branches? [release/]
Hotfix branches? [hotfix/]
Support branches? [support/]
Version tag prefix? []
Hooks and filters directory? [pathtofuseerepository/.git/hooks]
```

You can double check the configuration in `.git/config`, which should look like this:
```
[gitflow "branch"]
	master = master
	develop = develop
[gitflow "prefix"]
	feature = feature/
	bugfix = bugfix/
	release = release/
	hotfix = hotfix/
	support = support/
	versiontag = 
[gitflow "path"]
	hooks = pathtofuseerepository/.git/hooks
```

What is git-flow: https://nvie.com/posts/a-successful-git-branching-model/

Git-flow cheatsheet: https://danielkummer.github.io/git-flow-cheatsheet/
