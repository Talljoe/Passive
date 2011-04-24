Feature: DynamicModel Single()
  In order to access data
  As a developer
  I want to get a single row from the database

Background:
  Given a database with the following appliances
     | Id | Name      | Color           | Amps |
     | 1  | Toaster   | Stainless Steel | 7    |
     | 2  | Vacuum    | Red             | 12   |
     | 3  | Stove     | White           | 30   |
     | 4  | Microwave | White           | 20   |
     #-----------------------------------------#

Scenario: Getting a record without any filtering
  Given I have a model for Appliance
  When I ask for a single row
  Then I should get 1 result
  And it should be a subset of all data

Scenario: Getting a record by id
  Given I have a model for Appliance
  When I ask for the record with the id of 2
  Then I should get appliance #2

Scenario: Getting a record that doesn't exist
  Given I have a model for Appliance
  When I ask for the record with the id of 400
  Then I should get no results

Scenario Outline: Filtering records by an object
  Given I have a model for Appliance
  When I ask for a single row
  And I only want appliances colored <value>
  Then I should get <count> results
  And I should only get <value>-colored appliances

  Examples:
  | value           | count |
  | Stainless Steel | 1     |
  | Red             | 1     |
  | White           | 1     |
  | Green           | 0     |

Scenario Outline: Filtering records by string
  Given I have a model for Appliance
  When I ask for a single row
  And I only want appliances with more than <value> amps
  Then I should get <count> results
  And I should only get appliances with more than <value> amps

  Examples:
    | value | count |
    | 6     | 1     |
    | 7     | 1     |
    | 10    | 1     |
    | 15    | 1     |
    | 20    | 1     |
    | 30    | 0     |

Scenario: Selecting a subset of columns
  Given I have a model for Appliance
  When I ask for a single row
  And I ask for the columns "Id, Name"
  Then the records should only have the columns "Id, Name"

Scenario: Selecting an invalid column
  Given I have a model for Appliance
  When I ask for a single row
  And I ask for an invalid column
  Then the query should throw an exception