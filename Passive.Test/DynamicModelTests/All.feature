Feature: DynamicModel All
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


Scenario: Getting all records
  Given I have a model for Appliance
  When I ask for all rows
  Then I should get all items


Scenario: Getting some records
  Given I have a model for Appliance
  When I ask for 2 rows
  Then I should only have 2 results
  And they should be a subset of all data
