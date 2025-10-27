# Contributing to DynamicWpf

First off, thank you for considering contributing! This project is an open-source effort, and we welcome any help, from bug reports to new features.

## Code of Conduct

This project and everyone participating in it is governed by the [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs

* Ensure the bug was not already reported by searching on GitHub under [Issues](https://github.com/VerisFlow/DynamicUI/issues). 
* If you're unable to find an open issue addressing the problem, [open a new one](https://github.com/VerisFlow/DynamicUI/issues/new). Be sure to include a **title and clear description**, as much relevant information as possible, and a **code sample** or an **executable test case** demonstrating the expected behavior that is not occurring.
* Use the **Bug Report** template when creating the issue.

### Suggesting Enhancements

* Read the documentation to see if your idea is already part of the project.
* Suggest your new feature by [opening a new issue](https://github.com/VerisFlow/DynamicUI/issues/new).
* Use the **Feature Request** template when creating the issue. This will help you structure your idea.

### Pull Requests

We welcome pull requests for bug fixes and new features.

1.  **Fork** the repository and create your branch from `main`.
2.  **Set up your environment:**
    * You will need [Visual Studio 2022 (or later)](https://visualstudio.microsoft.com/vs/).
    * You must have the **.NET 8.0 SDK** installed, including the **.NET desktop development** workload (for WPF).
3.  **Build the project:** Open the `DynamicWpfFromConfig.sln` file and build the solution to ensure all dependencies are restored.
4.  **Make your changes:** Make your code changes in your new branch.
5.  **Test your changes:** Ensure the solution builds successfully in both `Debug` and `Release` modes. If you added new functionality, please test it.
6.  **Update documentation:** If your change affects the `ui-config.json` schema or adds/changes functionality, update the relevant `README.md` file.
7.  **Submit your Pull Request:** Push your changes to your fork and submit a pull request against the `main` branch. Provide a clear description of the problem and your solution.