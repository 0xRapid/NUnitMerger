using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NUnitMerger.Versions
{
	public class NUnit3
	{
		//Merger for NUnit 3.10 TestResult.xml files
		public static bool MergeFiles(IEnumerable<string> files, string output)
		{
			XElement environment = null;
			XElement culture = null;
			var suites = new List<XElement>();

			bool finalSuccess = true;
			string finalResult = "";
			double duration = 0;
			//Test Run Attributes
			int total = 0, passed = 0, failed = 0, inconclusive = 0, skipped = 0, asserts = 0;

			//TestCaseCount is currently not being used, since it's the same like total.
			foreach (var file in files)
			{
				var doc = XDocument.Load(file);
				XElement testRun = doc.Element("test-run");
				IEnumerable<XElement> testSuites = testRun.Elements("test-suite");

				foreach (var testSuite in testSuites)
				{


					if (environment == null)
					{
						environment = testSuite.Element("environment");
					}

					total += Convert.ToInt32(testRun.Attribute("total").Value);
					passed += Convert.ToInt32(testRun.Attribute("passed").Value);
					failed += Convert.ToInt32(testRun.Attribute("failed").Value);
					inconclusive += Convert.ToInt32(testRun.Attribute("inconclusive").Value);
					skipped += Convert.ToInt32(testRun.Attribute("skipped").Value);
					asserts += Convert.ToInt32(testRun.Attribute("asserts").Value);

					string result = testSuite.Attribute("result").Value;
					bool isPassed = bool.TryParse(testSuite.Attribute("passed").Value, out bool isPassedValue);
					if (!isPassedValue)
					{
						finalSuccess = false;
					}

					duration += Convert.ToDouble(testSuite.Attribute("duration").Value);

					if (finalResult != "Failed" && (String.IsNullOrEmpty(finalResult) || result == "Failed" || finalResult == "Success"))
						finalResult = result;

					suites.Add(testSuite);
				}
			}

			if (String.IsNullOrEmpty(finalResult))
			{
				finalSuccess = false;
				finalResult = "Inconclusive";
			}

			var project = XElement.Parse(
				$"<test-suite type=\"Automation Tests\" name=\"Name\" executed=\"True\" result=\"{finalResult}\" success=\"{(finalSuccess ? "True" : "False")}\" time=\"{duration}\" />");
			var results = XElement.Parse("<results/>");
			results.Add(suites.ToArray());
			project.Add(results);

			var now = DateTime.Now;
			//The total results.
			var trfinal = XElement.Parse(
				$"<test-run name=\"Merged Automation Tests\" testcasecount=\"{total}\" total=\"{total}\" failures=\"{failed}\" passed=\"{passed}\" inconclusive=\"{inconclusive}\" skipped=\"{skipped}\" asserts=\"{asserts}\" date=\"{now:yyyy-MM-dd}\" time=\"{now:HH:mm:ss}\" />");
			trfinal.Add(new[] {environment, project});
			trfinal.Save(output);

			return finalSuccess;
		}
	}
}