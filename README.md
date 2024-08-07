# Code Challenge

## File Generation Utility
### Functional Requirements
- Create a utility that generates a file
  - the file business part of the file is split by newline
  - each line has the format: <strong><em>"[Number][. ][Text]"</em></strong>
- You must supply a file size in bytes
- There should be some percentage of lines with the same text and different numeric part
- There should be some percentage of duplicated lines

### Non-Functional Requirements
- Optimize the generation to be as fast as possible. (Batching write)
- The last line should be valid content

## File Sort Utility
### Functional Requirements
- Sort a file by the <ins><strong><em>Text</em></strong> component first</ins>, and <ins>then by the <strong><em>Number</em></strong> component</ins>
- Write the sorted output to a new file
- Report the time it took to process all
- Repeated lines must be kept in the output

### Non-Functional Requirements
- the whole process must be timed to report in the end
- The whole process (loading + sorting + saving) should take less than a minute. 1 minute is
  the max time.
- it should process 1 min /1 GB , file size up to 100GB.
- Any string expression separated by spaces should be counted as valid.
- the task should do sort and print time.

## Assumptions
- Lets assume that the generation and the processing is done in the same environment. The [newline](https://en.wikipedia.org/wiki/Newline) will be managed by the dotnet property [Environment.NewLine](https://learn.microsoft.com/en-us/dotnet/api/system.environment.newline?view=net-8.0).

## Out of scope
