# Code Challenge

## Context
The specific .NET applications sorts files with the following format:

    415. Apple
    30432. Something something something
    1. Apple
    32. Cherry is the best
    2. Banana is yellow

Each record contains number and corresponding string separated by dot and space.
At first the application compares the string part of different records and in the case of their equality it compares numbers related to each string. So the output  file in the example above will be the following:

    1. Apple
    415. Apple
    2. Banana is yellow
    32. Cherry is the best
    30432. Something something something


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

## Validation
### Generator
#### Line Duplication
```bash
wc -l outputfile.txt
```
vs
```bash
sort outputfile.txt | uniq | wc -l
```

#### Sentence Duplication
```bash
sort out2.txt | uniq | awk '
BEGIN { line_count = 0 }
{
    # Find the first occurrence of ". " and extract the rest of the line
    split($0, parts, /\. /)
    if (length(parts) > 1) {
        # Print the extracted part after ". "
        print substr($0, index($0, ". ") + 2)
        line_count++
    }
}
' | wc -l
```

vs

```bash
sort out2.txt | uniq | awk '
BEGIN { line_count = 0 }
{
    # Find the first occurrence of ". " and extract the rest of the line
    split($0, parts, /\. /)
    if (length(parts) > 1) {
        # Print the extracted part after ". "
        print substr($0, index($0, ". ") + 2)
        line_count++
    }
}
' | sort | uniq | wc -l


```


# References
1. https://github.com/Dobby007/External-Sorting-Algorithm
2. https://github.com/vgotra/LargeTextFilesSorting
3. https://en.wikipedia.org/wiki/External_sorting
4. https://medium.com/@mbanaee61/a-deep-dive-into-merge-algorithms-unraveling-the-magic-of-external-sorting-with-a-sample-69dd2abf4316


