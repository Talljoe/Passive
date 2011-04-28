@async
Feature: DynamicAsyncModel All()
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

Scenario Outline: Getting some records
  Given I have a model for Appliance
  When I ask for all rows
  And I limit the query to <n> rows
  Then I should only have <n> results
  And they should be a subset of all data

  Examples:
    |n|
    |1|
    |2|
    |3|
    |4|

Scenario: Asking for too many records
  Given I have a model for Appliance
  When I ask for all rows
  And I limit the query to more rows than are in the database
  Then I should get all items

Scenario Outline: Filtering records by an object
  Given I have a model for Appliance
  When I ask for all rows
  And I only want appliances colored <value>
  Then I should only have <count> results
  And I should only get <value>-colored appliances

  Examples:
  | value           | count |
  | Stainless Steel | 1     |
  | Red             | 1     |
  | White           | 2     |
  | Green           | 0     |

Scenario Outline: Filtering records by string
  Given I have a model for Appliance
  When I ask for all rows
  And I only want appliances with more than <value> amps
  Then I should only have <count> results
  And I should only get appliances with more than <value> amps

  Examples:
    | value | count |
    | 6     | 4     |
    | 7     | 3     |
    | 10    | 3     |
    | 15    | 2     |
    | 20    | 1     |
    | 30    | 0     |


Scenario Outline: Executing a query with order by
  Given I have a model for Appliance
  When I ask for all rows
  And I order rows by <orderby>
  Then the records should be sorted by <orderby>

  Examples:
  | orderby   |
  | id        |
  | name      |
  | AMPS      |

Scenario Outline: Executing a query with descending order by
  Given I have a model for Appliance
  When I ask for all rows
  And I order rows by <orderby> desc
  Then the records should be reverse-sorted by <orderby>

  Examples:
  | orderby |
  | id      |
  | name    |
  | AMPS    |

Scenario: Selecting a subset of columns
  Given I have a model for Appliance
  When I ask for all rows
  And I ask for the columns "Id, Name"
  Then the records should only have the columns "Id, Name"

Scenario: Selecting an invalid column
  Given I have a model for Appliance
  When I ask for all rows
  And I ask for an invalid column
  Then the query should throw an exception