Feature: DynamicModel All()
  In order to access data
  As a developer
  I want to get all rows from the database

Background:
  Given a database with the following appliances
     | Id | Name      | Color           | Amps |
     | 1  | Toaster   | Stainless Steel | 7    |
     | 2  | Vacuum    | Red             | 12   |
     | 3  | Stove     | White           | 30   |
     | 4  | Microwave | White           | 20   |
     #-----------------------------------------#

Scenario: Getting all records
  Given I have a model for Appliance
  When I ask for all rows
  Then I should get all items

Scenario: Getting some records
  Given I have a model for Appliance
  When I ask for 2 rows
  Then I should only have 2 results
  And they should be a subset of all data

Scenario: Asking for too many records
  Given I have a model for Appliance
  When I ask for more rows than are in the database
  Then I should get all items

Scenario: Filtering records by an object
  Given I have a model for Appliance
  When I ask for appliances colored White
  Then I should only have 2 results
  And I should only get White-colored appliances

Scenario: Filtering records by string
  Given I have a model for Appliance
  When I ask for appliances with more than 10 amps
  Then I should only have 3 results
  And I should only get appliances with more than 10 amps

Scenario: Executing a query with order by
  Given I have a model for Appliance
  When I order rows by Amps
  Then the records should be sorted by Amps

Scenario: Executing a query with descending order by
  Given I have a model for Appliance
  When I order rows by Id desc
  Then the records should be reverse-sorted by Id

Scenario: Selecting a subset of columns
  Given I have a model for Appliance
  When I ask for the columns "Id, Name"
  Then the records should only have the columns "Id, Name"

Scenario: Selecting an invalid column
  Given I have a model for Appliance
  When I ask for an invalid column
  Then the query should throw an exception