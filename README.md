# Dotnet console app to automate testing for [MasterThesis](https://github.com/Dovrol/MasterThesis) project

This console app connects to [MasterThesis](https://github.com/Dovrol/MasterThesis API and provide testing for selected operation.

> Testing means executing same operation 10 times and saving collected data.  

## How to run:
Download this repository and execute dotnet run *[operation]*[^1] *[output_result_path]*

[^1]: Supported operations are listed [here](#operations) 

## Output
App is saving CSV file with results in specified *[output_result_path]*


## Supported oparations: {#operations}
* create
* update
* delete
* queryOne
* queryTwo
* queryThree
* queryFour
* queryFive



