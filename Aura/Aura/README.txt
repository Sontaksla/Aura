Aura is a version control system inspired by git.

  Some common CLI commands:

	Create a fresh new repository (created by default every first command)
		>aura create

	Get info about the whole project
		>aura info

	Get info about current branch
		>aura branch -i
	
	Make a commit to the current branch
		>aura commit -m "message"
	
	Create new branch
		>aura branch -n myBrach

	Switch to the new branch
		>aura branch myBranch

	Merge two branches
		>aura branch -m master myBranch

For more information
	>aura help