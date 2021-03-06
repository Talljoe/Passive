// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.6.1.0
//      SpecFlow Generator Version:1.6.0.0
//      Runtime Version:4.0.30319.235
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
namespace Passive.Test.DynamicModelTests
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.6.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class DynamicModelAllFeature : Xunit.IUseFixture<DynamicModelAllFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "All.feature"
#line hidden
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "DynamicModel All()", "In order to access data\r\nAs a developer\r\nI want to get all rows from the database" +
                    "", GenerationTargetLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public virtual void SetFixture(DynamicModelAllFeature.FixtureData fixtureData)
        {
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
            this.FeatureBackground();
        }
        
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        void System.IDisposable.Dispose()
        {
            this.ScenarioTearDown();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "Name",
                        "Color",
                        "Amps",
                        "In Stock"});
            table1.AddRow(new string[] {
                        "1",
                        "Toaster",
                        "Stainless Steel",
                        "7",
                        "false"});
            table1.AddRow(new string[] {
                        "2",
                        "Vacuum",
                        "Red",
                        "12",
                        "true"});
            table1.AddRow(new string[] {
                        "3",
                        "Stove",
                        "White",
                        "30",
                        "true"});
            table1.AddRow(new string[] {
                        "4",
                        "Microwave",
                        "White",
                        "20",
                        "false"});
#line 7
  testRunner.Given("a database with the following appliances", ((string)(null)), table1);
#line hidden
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Getting all records")]
        public virtual void GettingAllRecords()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Getting all records", ((string[])(null)));
#line 15
this.ScenarioSetup(scenarioInfo);
#line 16
  testRunner.Given("I have a model for Appliance");
#line 17
  testRunner.When("I ask for all rows");
#line 18
  testRunner.Then("I should get all items");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Getting some records")]
        [Xunit.Extensions.InlineDataAttribute("1", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("2", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("3", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("4", new string[0])]
        public virtual void GettingSomeRecords(string n, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Getting some records", exampleTags);
#line 20
this.ScenarioSetup(scenarioInfo);
#line 21
  testRunner.Given("I have a model for Appliance");
#line 22
  testRunner.When("I ask for all rows");
#line 23
  testRunner.And(string.Format("I limit the query to {0} rows", n));
#line 24
  testRunner.Then(string.Format("I should only have {0} results", n));
#line 25
  testRunner.And("they should be a subset of all data");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Asking for too many records")]
        public virtual void AskingForTooManyRecords()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Asking for too many records", ((string[])(null)));
#line 34
this.ScenarioSetup(scenarioInfo);
#line 35
  testRunner.Given("I have a model for Appliance");
#line 36
  testRunner.When("I ask for all rows");
#line 37
  testRunner.And("I limit the query to more rows than are in the database");
#line 38
  testRunner.Then("I should get all items");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Filtering records by an object")]
        [Xunit.Extensions.InlineDataAttribute("Stainless Steel", "1", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("Red", "1", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("White", "2", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("Green", "0", new string[0])]
        public virtual void FilteringRecordsByAnObject(string value, string count, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Filtering records by an object", exampleTags);
#line 40
this.ScenarioSetup(scenarioInfo);
#line 41
  testRunner.Given("I have a model for Appliance");
#line 42
  testRunner.When("I ask for all rows");
#line 43
  testRunner.And(string.Format("I only want appliances colored {0}", value));
#line 44
  testRunner.Then(string.Format("I should only have {0} results", count));
#line 45
  testRunner.And(string.Format("I should only get {0}-colored appliances", value));
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Filtering records by string")]
        [Xunit.Extensions.InlineDataAttribute("6", "4", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("7", "3", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("10", "3", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("15", "2", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("20", "1", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("30", "0", new string[0])]
        public virtual void FilteringRecordsByString(string value, string count, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Filtering records by string", exampleTags);
#line 54
this.ScenarioSetup(scenarioInfo);
#line 55
  testRunner.Given("I have a model for Appliance");
#line 56
  testRunner.When("I ask for all rows");
#line 57
  testRunner.And(string.Format("I only want appliances with more than {0} amps", value));
#line 58
  testRunner.Then(string.Format("I should only have {0} results", count));
#line 59
  testRunner.And(string.Format("I should only get appliances with more than {0} amps", value));
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Executing a query with order by")]
        [Xunit.Extensions.InlineDataAttribute("id", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("name", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("AMPS", new string[0])]
        public virtual void ExecutingAQueryWithOrderBy(string orderby, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Executing a query with order by", exampleTags);
#line 71
this.ScenarioSetup(scenarioInfo);
#line 72
  testRunner.Given("I have a model for Appliance");
#line 73
  testRunner.When("I ask for all rows");
#line 74
  testRunner.And(string.Format("I order rows by {0}", orderby));
#line 75
  testRunner.Then(string.Format("the records should be sorted by {0}", orderby));
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Executing a query with descending order by")]
        [Xunit.Extensions.InlineDataAttribute("id", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("name", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("AMPS", new string[0])]
        public virtual void ExecutingAQueryWithDescendingOrderBy(string orderby, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Executing a query with descending order by", exampleTags);
#line 83
this.ScenarioSetup(scenarioInfo);
#line 84
  testRunner.Given("I have a model for Appliance");
#line 85
  testRunner.When("I ask for all rows");
#line 86
  testRunner.And(string.Format("I order rows by {0} desc", orderby));
#line 87
  testRunner.Then(string.Format("the records should be reverse-sorted by {0}", orderby));
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Selecting a subset of columns")]
        public virtual void SelectingASubsetOfColumns()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Selecting a subset of columns", ((string[])(null)));
#line 95
this.ScenarioSetup(scenarioInfo);
#line 96
  testRunner.Given("I have a model for Appliance");
#line 97
  testRunner.When("I ask for all rows");
#line 98
  testRunner.And("I ask for the columns \"Id, Name\"");
#line 99
  testRunner.Then("the records should only have the columns \"Id, Name\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "DynamicModel All()")]
        [Xunit.TraitAttribute("Description", "Selecting an invalid column")]
        public virtual void SelectingAnInvalidColumn()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Selecting an invalid column", ((string[])(null)));
#line 101
this.ScenarioSetup(scenarioInfo);
#line 102
  testRunner.Given("I have a model for Appliance");
#line 103
  testRunner.When("I ask for all rows");
#line 104
  testRunner.And("I ask for an invalid column");
#line 105
  testRunner.Then("the query should throw an exception");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.6.1.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                DynamicModelAllFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                DynamicModelAllFeature.FeatureTearDown();
            }
        }
    }
}
#endregion
