Feature: Trace queries
  In order to debug my application
  As a developer
  I want to be able to see what SQL is being executed

Scenario: Trace events are called on query
  Given a database
  When I execute the query "SELECT 'Hello' as [Greeting], 'World' as [Subject]"
  Then I should get a BeginQuery event with the value "SELECT 'Hello' as [Greeting], 'World' as [Subject]"
  And I should get an EndQuery event with the value "SELECT 'Hello' as [Greeting], 'World' as [Subject]"

Scenario: Trace events are called on fetch
  Given a database
  When I fetch the query "SELECT 'Hello' as [Greeting], 'World' as [Subject]"
  Then I should get a BeginQuery event with the value "SELECT 'Hello' as [Greeting], 'World' as [Subject]"
  And I should get an EndQuery event with the value "SELECT 'Hello' as [Greeting], 'World' as [Subject]"

Scenario: Trace events are called on scalar
  Given a database
  When I execute the scalar "SELECT 1 as [Id]"
  Then I should get a BeginQuery event with the value "SELECT 1 as [Id]"
  And I should get an EndQuery event with the value "SELECT 1 as [Id]"

Scenario: Trace events are called on execute
  Given a database
  When I execute "SELECT 1 as [Id]"
  Then I should get a BeginQuery event with the value "SELECT 1 as [Id]"
  And I should get an EndQuery event with the value "SELECT 1 as [Id]"

Scenario: Trace events are called on multiple execute
  Given a database
  When I execute
   |SELECT 1 as [Id]|
   |SELECT 2 as [Id]|
   |SELECT 3 as [Id]|
  Then I should get a BeginQuery event with the values
   |SELECT 1 as [Id]|
   |SELECT 2 as [Id]|
   |SELECT 3 as [Id]|
  And I should get an EndQuery event with the values
   |SELECT 1 as [Id]|
   |SELECT 2 as [Id]|
   |SELECT 3 as [Id]|
