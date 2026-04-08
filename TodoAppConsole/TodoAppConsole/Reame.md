Assignment: Simple File‑Based Notes App
 
1. Objective
This version enhances the Notes App to simulate a real to‑do list, focusing on:
Status management (Done / Not Done)
File parsing and rewriting
Filtering data based on user options
Maintaining data consistency
Learners will practice:
File I/O
Lists and conditions
State updates in persistent storage
 
2. Application Overview
A console-based Notes / To‑Do application that allows users to:
Add notes
Mark a note as Done
View: 
All notes
Only notes not done or done
Delete a note
Clear all notes
All data must persist via a file
 
3. Note Data Model (Logical)
Each note logically contains:
Field 	Description 
Id Auto-increment number 
CreatedAt 
Content Note text 
Status DONE / NOT_DONE 
 
4. File Storage Format
Each note must include status:
[ID: 1] [2026-04-07 16:30:10] [Status: NOT_DONE] 
Prepare C# training materials 
---------------------------------- 
 
When a note is marked done:
[ID: 1] [2026-04-07 16:30:10] [Status: DONE] 
 
Status must be written to file 
Separator line is mandatory
 
5. Console Menu
=== Notes Application === 
1. Add a new note 
2. View notes 
3. Mark note as done 
4. Delete a note by ID 
5. Clear all notes 
6. Exit 
 

6. Functional Requirements
6.1 Add a New Note
User enters note content
Content must not be empty
New note properties: 
Auto‑generated ID
Current created at
Initial status = NOT_DONE
Append note to file
 
6.2 View Notes (FILTERED VIEW)
When user selects View Notes, prompt:
1. View all notes 
2. View only NOT DONE notes 
Choose an option: 
 
Option 1 – View All
Display all notes regardless of status
Option 2 – View Only Not Done
Display only notes with: Status: NOT_DONE
Notes must show:
ID
created at
Status
Content

If no notes match filter:
No notes found. 
 
6.3 Mark Note as Done
User Flow
Display only NOT_DONE notes
Prompt: Enter the ID of the note to mark as done:
Validate: 
ID is numeric
ID exists
Note is not already DONE
Confirm: Are you sure you want to mark this note as DONE? (Y/N)
On confirmation: 
Update note status to DONE
Rewrite file with updated data
 
Required Logic
Read all notes from file
Parse notes into a list
Find note by ID
Change status to DONE
Rewrite entire file
 
6.4 Delete One Note
Delete note by ID
Confirm before deleting
Rewrite file
Reassign IDs sequentially
 
6.5 Clear All Notes
Require confirmation
Clear file content
 
7. Validation & Error Handling
Scenario 
	
Expected Behavior 


Empty note 
	
Reject input 


Non-numeric ID 
	
Show error 


ID not found 
	
Show message 


Mark DONE again 
	
Show warning 


File missing 
	
Auto-create file 


IO exception 
	
Handle with try‑catch 
 
8. Suggested Helper Methods
ShowMenu()
AddNote()
ViewNotes()
MarkNoteAsDone()
DeleteNoteById()
ClearNotes()
LoadNotesFromFile()
SaveNotesToFile()
 
9. Sample Console Interaction
View options: 
1. View all notes 
2. View only NOT DONE notes 
Choose: 2 
 
ID: 3 | Status: NOT_DONE 
Prepare demo for class 
---------------------------------- 
 
Enter the ID to mark as done: 3 
Are you sure? (Y/N): Y 
 
Note marked as DONE. 
 

11. Optional Enhancements (Advanced)
Use enum NoteStatus
Search by keyword
Undo mark as done
Store notes as JSON
Add sorting (by date or status)
Conversion to OOP with a Note class
Using LINQ
Business logic must not be inside Main() only
Keep methods small and readable