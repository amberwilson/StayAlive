using System;
using NUnit.Framework;

namespace StayAlive
{
  public class Tests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestStarvedMultipliers()
    {
      // init person to be about to starve
      Person person = new Person(PersonActivity.Idle, new StateFactors(1, 50, 50, 50));

      Experimentor experimentor = new Experimentor(new ExperimentorConfig(), person);

      experimentor.ApplyStateToPerson();
      experimentor.ApplyStateToPerson();
      experimentor.ApplyStateToPerson();

      Console.WriteLine($"{experimentor.Person}");

      Assert.AreEqual(0f, experimentor.Person.State.Satiation);
      Assert.AreEqual(46, experimentor.Person.State.Weight);
      Assert.IsTrue(experimentor.Person.ConditionFlags[PersonCondition.Starved]);

      // check that Hydration kept going down when Starving kicked in
      Assert.AreEqual(47.0f, experimentor.Person.State.Hydration);
    }
  }
}