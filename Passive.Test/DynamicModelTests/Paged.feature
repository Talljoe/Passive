Feature: DynamicModel Paged()
  In order to reduce the load on my database
  As a developer
  I want to get page results

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
  When I ask for page 1
  And the page size is 20
  Then I should get all items

Scenario Outline: Getting some records
  Given I have a model for Appliance
  When I ask for page <page>
  And the page size is <page size>
  Then I should get <count> results
  And they should be a subset of all data

  Examples:
    | page | page size | count |
    | 1    | 2         | 2     |
    | 2    | 2         | 2     |
    | 3    | 2         | 0     |
    | 1    | 3         | 3     |
    | 2    | 3         | 1     |
    | 3    | 3         | 0     |
    | 1    | 4         | 4     |
    | 2    | 4         | 0     |

Scenario Outline: Filtering records by an object
  Given I have a model for Appliance
  When I ask for page 1
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
  When I ask for page 1
  And the page size is 2
  And I only want appliances with more than <value> amps
  Then I should only have <count> results
  And I should only get appliances with more than <value> amps

  Examples:
    | value | count |
    | 6     | 2     |
    | 7     | 2     |
    | 10    | 2     |
    | 15    | 2     |
    | 20    | 1     |
    | 30    | 0     |


Scenario Outline: Executing a query with order by
  Given I have a model for Appliance
  When I ask for page 1
  And I order rows by <orderby>
  Then the records should be sorted by <orderby>

  Examples:
  | orderby   |
  | id        |
  | name      |
  | AMPS      |

Scenario Outline: Executing a query with descending order by
  Given I have a model for Appliance
  When I ask for page 1
  And I order rows by <orderby> desc
  Then the records should be reverse-sorted by <orderby>

  Examples:
  | orderby |
  | id      |
  | name    |
  | AMPS    |

Scenario: Selecting a subset of columns
  Given I have a model for Appliance
  When I ask for page 1
  And I ask for the columns "Id, Name"
  Then the records should only have the columns "Id, Name"

Scenario: Selecting an invalid column
  Given I have a model for Appliance
  When I ask for page 1
  And I ask for an invalid column
  Then the query should throw an exception