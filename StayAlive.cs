using System;

namespace StayAlive {
  public static class StringHelper {
    public static string KeepSize (string str, int size = 6) {
      char x = 'x';
      return str.PadRight (size - str.Length, x);
    }
  }

  /// <summary>
  /// Tracks rate of change or current state
  /// </summary>
  public class StateFactors {
    private float _satiation;
    public float Satiation {
      get => _satiation;
      set {
        CheckFactorSet (_satiation, value, new StarvedException (), new StuffedException ());
        _satiation = value;
      }
    }

    private float _hydration;
    public float Hydration {
      get => _hydration;
      set {
        CheckFactorSet (_hydration, value, new ParchedException (), new SaturatedException ());
        _hydration = value;
      }
    }

    private float _energy;
    public float Energy {
      get => _energy;
      set {
        CheckFactorSet (_energy, value, new DrainedException (), new CrankedException ());
        _energy = value;
      }
    }

    private float _weight;
    public float Weight {
      get => _weight;
      set {
        CheckFactorSet (_weight, value, new StarvedException (), new StuffedException ());
        _weight = value;
      }
    }

    public StateFactors (float satiation, float hydration, float energy, float weight) {
      this.Satiation = satiation;
      this.Hydration = hydration;
      this.Energy = energy;
      this.Weight = weight;
    }

    private static void CheckFactorSet (float currentValue, float newValue, Exception lowException, Exception highException) {
      if (currentValue > 100f && newValue > 0f) {
        throw highException;
      }

      if (currentValue < 0f && newValue < 0f) {
        throw lowException;
      }
    }

    public override string ToString () {
      return $"Hydration: {StringHelper.KeepSize(_hydration.ToString())}\t\tSatiation: {StringHelper.KeepSize(_satiation.ToString())}\t\tStamina: {StringHelper.KeepSize(_energy.ToString())}\t\tWeight: {StringHelper.KeepSize(_weight.ToString())}";
    }

    public void ToConsole () {
      Console.WriteLine (ToString ());
    }
  }

  public enum PersonState {
    Idle,
    Eating,
    Drinking,
    Napping,
    Sleeping,
    Walking,
    Running
  }

  public class Person {
    public PersonState state = PersonState.Idle;
    public StateFactors stateFactors = new StateFactors (100, 100, 100, 100);

    public void ToConsole () {
      Console.WriteLine ($"State: {state}\t{stateFactors}");
    }
  }

  public class Experimentor {
    public Person person = new Person ();
    public StateFactors IdleFactors = new StateFactors (-1, -1, 0, -0.5f);
    public StateFactors eatingFactors = new StateFactors (+1, -1, 0, +0.5f);
    public StateFactors drinkingFactors = new StateFactors (-1, 1, 0, -0.5f);
    public StateFactors nappingFactors = new StateFactors (-1, -1, +0.5f, -0.35f);
    public StateFactors sleepingFactors = new StateFactors (-1, -1, +1, -0.25f);
    public StateFactors walkingFactors = new StateFactors (-2.5f, -1.5f, -2, -1);
    public StateFactors runningFactors = new StateFactors (-5, -3, -4, -2);

    public Experimentor () {
      Console.WriteLine ($"String Experimentor - Peron's starting state is...\n{person.stateFactors}");
    }

    public void checkState () {
      StateFactors currentStateFactors = getFactorsForPersonState ();
      currentStateFactors.ToConsole ();
    }

    public StateFactors getFactorsForPersonState () {
      switch (person.state) {
        case PersonState.Idle:
          return IdleFactors;
        case PersonState.Eating:
          return eatingFactors;
        case PersonState.Drinking:
          return drinkingFactors;
        case PersonState.Napping:
          return nappingFactors;
        case PersonState.Sleeping:
          return sleepingFactors;
        case PersonState.Walking:
          return walkingFactors;
        case PersonState.Running:
          return runningFactors;
        default:
          throw new Exception ($"{person.state} is not a supported state");
      }
    }

    public void applyStateToPerson () {
      StateFactors currentStateFactors = getFactorsForPersonState ();

      Console.WriteLine ("\n\nApplying state...");
      Console.WriteLine ($"Current Factors: {currentStateFactors}");
      Console.WriteLine ($"Persons Factors: {person.stateFactors}");

      person.stateFactors.Satiation += currentStateFactors.Satiation;
      person.stateFactors.Hydration += currentStateFactors.Hydration;
      person.stateFactors.Energy += currentStateFactors.Energy;
      person.stateFactors.Weight += currentStateFactors.Weight;

      person.ToConsole ();
    }
  }

  public class StarvedException : Exception { }
  public class StuffedException : Exception { }

  public class ParchedException : Exception { }
  public class SaturatedException : Exception { }

  public class DrainedException : Exception { }
  public class CrankedException : Exception { }

  public class UnderweightException : Exception { }
  public class OverweightException : Exception { }

}