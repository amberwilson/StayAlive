using System;
using StayAlive;

class MainClass
{
  public static void Main(string[] args)
  {
    Experimentor experimentor = new Experimentor();
    experimentor.applyStateToPerson();
    experimentor.person.state = PersonState.Eating;
    experimentor.applyStateToPerson();
    experimentor.person.state = PersonState.Eating;
    experimentor.applyStateToPerson();
    experimentor.applyStateToPerson();
    experimentor.person.state = PersonState.Drinking;
    experimentor.applyStateToPerson();
    experimentor.applyStateToPerson();
    experimentor.person.state = PersonState.Napping;
    experimentor.applyStateToPerson();
    experimentor.applyStateToPerson();
    experimentor.person.state = PersonState.Running;
    experimentor.applyStateToPerson();
  }
}