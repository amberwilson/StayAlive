using System;
using StayAlive;

class MainClass
{
  public static void Main(string[] args)
  {
    Experimentor experimentor = new Experimentor();
    experimentor.ApplyStateToPerson();
    experimentor.ApplyStateToPerson();

    experimentor.UpdatePersonActivity("nap");
    experimentor.ApplyStateToPerson();
    experimentor.ApplyStateToPerson();

    experimentor.UpdatePersonActivity("drink");
    experimentor.ApplyStateToPerson();
    experimentor.ApplyStateToPerson();

    experimentor.UpdatePersonActivity("eat");
    experimentor.ApplyStateToPerson();
    experimentor.ApplyStateToPerson();

    experimentor.UpdatePersonActivity("relax");
    experimentor.ApplyStateToPerson();

  }
}