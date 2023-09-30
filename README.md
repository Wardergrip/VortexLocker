# VortexLocker

VortexLocker is a WPF program aimed to bring the benefits of file locking to git to help smaller teams working on game development.

## Why?
In essence, we are trying to prevent merge conflicts on files where they are hard or impossible to solve.
File locking is useful for working with multiple people on a project that has binary files that might have to be edited regularly. In some occasions, the order of changes can alter the end result. In this case it can also be handy to have the option to lock files to make sure that the next person works with the latest version.
File locking is not useful for a project where the big majority of changes are only in code files.

## How does this VortexLocker file locking system work?
When initializing the project with VortexLocker, all files will be marked as read only on your local machine and a `.vortex` file will be made in the root of the repository. This file is never read only. This file contains a list of git usernames and which file paths they have currently locked. 
Through the program you can lock files. You can only lock files if that file is not registered in the `.vortex` file in the root of the repository. 
Locking the file will write the file path under your username in the `.vortex` file and makes it writable, allowing you to make any changes.
It is important that when you are done locking and unlocking files that it is pushed to the remote repository as fast as possible since it communicates to others that you are working on said file and others cannot lock it for themselves.

## Downsides
Since changes to the `.vortex` file can only happen when a git commit happens, it means that the commit history will be cluttered and harder to use when tracing back when bugs or problems got introduced.
This method of working tries to solve merge conflicts by avoiding them, however this requires everyone to have the latest update of this file, all the time. This results in being limited in using git branches. This downside is not really that big if you use branching for new features however it is annoying if you want a stable main branch, developing branch, test branch and so on. A workaround is to have all "branches" as duplicates of the project in the repository.

## Current state of the program
The majority of the important UI is in place. A tree view displaying the project directories and files is in place. Locking files will successfully write to the `.vortex` file and you can commit the change from the program.
Currently, the UI does not update once you pull changes when the program is open or when you lock/unlock files yourself.
The program doesn't handle fetching and pulling changes well.

![image](https://github.com/Wardergrip/VortexLocker/assets/42802496/089af29f-8293-4c23-8023-9fb494ca9d58)
