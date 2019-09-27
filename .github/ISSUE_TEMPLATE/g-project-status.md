While this list provides a list of steps to do as a Project lead, it is in no way a substitute for the [Project leading guidelines](https://www.notion.so/toggl/Mobile-Project-Leading-fe1eaca52e46498d8575104bf018287b). You should be familiar with that document as well.

### 1. Prerequisites (in the weeks preceding the project start)
* [ ] 🧠 Accept that you are now a Project Manager and that you are responsible for the success of this project. *This is your project. There are many like it, but this one is yours*. Take a deep breath and let that sink in.
* [ ] Decide on the project emoji that will be used for this project. If at all possible, us the unicode character (such as ❤) and not the `:heart:` variant so that it can be easily recognizable and remain an emoji in all situations.
* [ ] 🅰 Rename the title of this checklist issue to: `<Emoji> Project: <ProjectName>`
	* example: `🎛 Project: Widgets`.
* [ ] 📚 Ensure that the specification documents are complete, correct and unambiguous. If need be, read the specs several times so that things don't fall through the cracks. Discuss all ambiguities or unknowns with the relevant parties until there is none.
* [ ] 🚧 If the project requires any architectural changes or refactoring, have a discussion with the architect to avoid going the wrong direction.
* [ ] ♻ If your project touches the sync algorithm in any way, make sure you discuss your needs with the Sync Manager.
* [ ] 🎨 Make sure that all designs your project requires are ready.

---

* [ ] ✅ Create the issues that will cover all that needs to be done by the project. Take great care to ensure that all the issues are unambiguous and correct. In addition, make sure that there's sufficient information in them so that the work on them can be efficient.
	* Make sure the issues are small enough so that the required time can be easily estimated and that the required solution would be easy to review. The goal is for the issues to not take longer than several hours. If it takes the whole day (or even more), then consider splitting into smaller parts. Use your best judgement here.
* [ ] ⌚ Estimate the require time for each issue. Then double-check those estimates with the rest of the project team to make sure that everyone agrees on them.
* [ ] 🕓 Schedule the project with the team lead, taking into account all the ongoing projects and the schedule of all participants.
* [ ] 📃 Update the [project page on Notion](https://www.notion.so/toggl/ad87bf4fd8114411980a988d38842b35?v=0814a8f583864e1ebaf3c7f29f3443f7). Use the project document template guidelines if you are unsure what to write. If the project page is missing, use the `New` button and create a new page by using `Project template (mobile)` template page.
	* Ask product manager to help with such docs if needed.

---

* [ ] 🌿 Create a project branch: `project/name`, such as `project/widgets`.
* [ ] ✨ Create a Github Project that will contain all the project issues.
* [ ] 🔻 Assign this checklist issue to the created project.
* [ ] 🔙 Create a PR from this branch into `develop` as soon as the first issue from the project gets squashed into the project branch. _Unfortunatelly, Github does not allow empty PRs, so we have to wait until there's at least a single change in the diff._
* [ ] 🔻 Assign the above mentioned PR to the Github project.
* [ ] 📃 Make sure that the Github projects's description contains:
	* [ ] The link to the Notion project page
	* [ ] The link to the Github project branch
	* [ ] Start and due date (copy from `Toggl Plan`)
* [ ] 🔳 Make sure the project contains all the necessary columns for the issues (according to what the project lead and all the involved developers find to be most effective).
* [ ] 📎 Make sure that all the related issues are assigned to the created project.
* [ ] 🔀 Order the issues the way you think is **most effective** in your current project team, but do not forget about the **critical path**. If possible, put less important features to the end of the queue, in case something needs to be cut because of the delays.

### 2. As the project starts
* [ ] 💬 Organize and execute a **kickoff meeting**. 
	* [ ] Invite all of the following (regardless of their RSVP)
	* Mandatory participants: You and all the devs that will work on the project. 
	* Optional, but highly recommended participants: Team lead, product manager, designers.
	* Consider whether there's any other person, not tied to a role, but still relevant to the project and invite them as well.
* [ ] 🧭  Chose the **metrics** by which you will gauge the success of the project. (You can probably coordinate this with the product manager)

### 3. Weekly responsibilities (check these often during the project)
* [ ] 🔙 Merge `develop` back into the project branch.
	* Since the project branch is protected, you will have to do it through a regular PR, as specified in these steps:
		* Create a branch `project/name-mergeback` from `project/name` 
		* Merge `develop` into `project/name-mergeback`
		* Create a PR from `project/name-mergeback` into `project/name`
	* Remember that the mergeback PR has to be `merged` into the project branch and not `squashed`, so that commits from the `develop` are not lost.
* [ ] 📱 Try to organize the work in such a way so that each week a working test build (containing things already done) can be created so that some of the testing  can be done in parallel. If it does not make sense to have only part of the feature, ignore this point.
* [ ] 📃 Go through the project page on Notion and make sure things are up to date there.
* [ ] 💬 Try to organize a meeting with the devs in the project to ensure that you are aware of what the exact status is but also to increase the awareness of the potential blockers. In organized teams, this can be avoided altogether if those things are communicated to the project lead as soon as those things appear.
* 📃 Twice a week, (preferrably Monday and Thursday), provide a status report in the `#mobile-updates` channel. Inform everyone about the current state of the project, potential blockers or anything else you find a valuable information.
	* During the Tuesday weekly meeting, you will provide a similar report.
	* Modify this list according to the project length and let it remind you to post the updates.
	* [ ] Week 1 - Early
	* [ ] Week 1 - Late
	* [ ] Week 2 - Early
	* [ ] Week 2 - Late
	* ...
	* [ ] Week N - Early
	* [ ] Week N - Late

### 4. After the project is implemented
* [ ] ✅ Test! Test! Test!
	* Besides you and your project team, feel free to employ all Togglrs to help with testing by reaching out on public channels.
* [ ] 🔙 When you are confident that the feature is behaving correctly and that all the features doing what they should be doing, prepare the project branch for the merge into `develop`. This most likely requires a mergeback and conflict resolution.
	* [ ] Coordinate with the teamlead to find the right moment for merging the 
project PR into `develop`. The teamlead gives the green light in this case.
* [ ] Merge (not squash) the project PR.
* [ ] 💬 Organize a **retrospective meeting**
	* Participants list is the same as in `kickoff` meeting. Invite them all.
	* Discuss the good, the bad and the ugly of the project. Discuss also the potential future of the project and evaluate your project throught the metrics chosen at the beginning of the project. Make sure that someone from the Product is there because they should be as involved in this as you are.
* [ ] 📃 Write a **report** with [similar details](https://www.notion.so/toggl/Mobile-Project-Leading-fe1eaca52e46498d8575104bf018287b#5bc1cf7781e84c699c6fd642ad4775d9) as in the retrospective meeting.

### 5. Aftermath
* [ ] ⚡ Remember to keep an eye on how the users interact with the new features
* [ ] 📃 Write a follow-up document when you see the influence of the feature on our users

### 6. Crisis management
⚠ This section is activated when you realize that the deadline is no longer attainable (regardless of the reasons). However, you should be observant enough to anticipate potential delays ahead of time.

* [ ] Inform the teamlead and the product manager immediatelly

SCENARIO 1 - If the project can be completed partially
* [ ] If there are possible features you can cut from your project, that should be step 1. Carefully weigh down what can be cut and make a decision. If you need help, don't hesitate to contact the teamlead for guidance.
* [ ] When you have decided what to do, inform all the involved parties (the teamlead, the product manager and other stakeholders)

SCENARIO 2 - If cutting does not make sense within the context of the project
* [ ] If the delay seems necessary, discuss it with the teamlead as soon as possible. And in this case `as soon as possible` really means **as soon as possible**.
* [ ] In cooperation with the teamlead, decide how to proceed further.
* [ ] When teamlead blesses the recovery strategy, continue as decided.
