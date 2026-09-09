# Repository and distribution

Repository: https://github.com/saaeiddev/Galaxy-Velocity-Combat-Racers

The owner created the repository after the first authoring session. The source upload preserves the original README commit as its parent. The repository contains the Unity project, offline browser companion, test suite and documentation.

## Download and play

Use GitHub **Code → Download ZIP**, extract it, and open `Galaxy-Velocity-Play.html` or `Web/index.html` in a desktop browser with WebGL enabled. GitHub's source viewer does not run HTML. No Pages site or Windows executable is claimed.

## Clone

```sh
git clone https://github.com/saaeiddev/Galaxy-Velocity-Combat-Racers.git
cd Galaxy-Velocity-Combat-Racers
node --test Tests/core.test.cjs
```

Open the cloned folder through Unity Hub to import and build. See the README and validation report for the remaining Unity verification work.

## Earlier archive

The previously supplied downloadable archive contains `Repository.bundle`, preserving two local authoring commits. That bundle is not needed for a normal clone and is not duplicated in this repository. The `publish-github.ps1` helper is for creating a new repository from an archive; it intentionally refuses to overwrite an existing one.
