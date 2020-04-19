using System;
using System.Collections.Generic;

namespace StayAlive
{
  public class Logger
  {
    public Boolean DebuggerOn;

    public Logger(Boolean debuggerOn)
    {
      DebuggerOn = debuggerOn;
    }

    public void Debug(string message)
    {
      if (DebuggerOn)
      {
        // UnitEngine.Debug.Log(message); // uncomment this for unity
        Console.WriteLine(message); // comment this for unity
      }
    }
  }

  public static class StringHelper
  {
    public static string KeepSize(string str, int size = 6)
    {
      return str.PadRight(size - str.Length);
    }

    public static string KeepSize(float input, int size = 6)
    {
      int extraLength = 0;
      if (input < 0)
      {
        extraLength = 1;
      }

      string str = input.ToString();

      return str.PadRight(size - str.Length + extraLength);
    }
  }

  /// <summary>
  /// Tracks rate of change or current state
  /// </summary>
  public class StateFactors
  {
    private float _satiation;
    public float Satiation
    {
      get => _satiation;
      set
      {
        CheckFactorSet(_satiation, value, new StarvedException(), new StuffedException());
        _satiation = value;
      }
    }

    private float _hydration;
    public float Hydration
    {
      get => _hydration;
      set
      {
        CheckFactorSet(_hydration, value, new ParchedException(), new SaturatedException());
        _hydration = value;
      }
    }

    private float _energy;
    public float Energy
    {
      get => _energy;
      set
      {
        CheckFactorSet(_energy, value, new DrainedException(), new CrankedException());
        _energy = value;
      }
    }

    private float _weight;
    public float Weight
    {
      get => _weight;
      set
      {
        CheckFactorSet(_weight, value, new StarvedException(), new StuffedException());
        _weight = value;
      }
    }

    public StateFactors(float satiation, float hydration, float energy, float weight)
    {
      this.Satiation = satiation;
      this.Hydration = hydration;
      this.Energy = energy;
      this.Weight = weight;
    }

    private static void CheckFactorSet(float currentValue, float newValue, Exception lowException, Exception highException)
    {
      if (currentValue > 100f && newValue > 0f)
      {
        throw highException;
      }

      if (currentValue < 0f && newValue < 0f)
      {
        throw lowException;
      }
    }

    public override string ToString()
    {
      return $"Hydration: {StringHelper.KeepSize(_hydration)}\t\tSatiation: {StringHelper.KeepSize(_satiation)}\t\tStamina: {StringHelper.KeepSize(_energy)}\t\tWeight: {StringHelper.KeepSize(_weight)}";
    }
  }

  public enum PersonActivity
  {
    Idle,
    Eating,
    Drinking,
    Napping,
    Sleeping,
    Walking,
    Running,
    GodMode
  }

  public class Person
  {
    public PersonActivity Activity;
    public StateFactors State;

    public Person(PersonActivity activity, StateFactors state)
    {
      Activity = activity;
      State = state;
    }

    override public string ToString()
    {
      return $"Activity: {Activity} {State}";
    }
  }

  public class ExperimentorConfig
  {
    public bool DebuggerOn = true;
    public PersonActivity DefaultPersonActivity = PersonActivity.Idle;
    public StateFactors DefaultPersonState = new StateFactors(50, 50, 50, 59);
    public StateFactors StateFactorMaximums = new StateFactors(100, 100, 100, 100);
    public StateFactors IdleFactors = new StateFactors(-1, -1, 0, -0.5f);
    public StateFactors EatingFactors = new StateFactors(+1, -1, 0, +0.5f);
    public StateFactors DrinkingFactors = new StateFactors(-1, 1, 0, -0.5f);
    public StateFactors NappingFactors = new StateFactors(-1, -1, +0.5f, -0.35f);
    public StateFactors SleepingFactors = new StateFactors(-1, -1, +1, -0.25f);
    public StateFactors WalkingFactors = new StateFactors(-2.5f, -1.5f, -2, -1);
    public StateFactors RunningFactors = new StateFactors(-5, -3, -4, -2);
    public Dictionary<string, PersonActivity> AcivityMap = new Dictionary<string, PersonActivity>(){
      {"relax", PersonActivity.Idle},
      {"drink", PersonActivity.Drinking},
      {"eat", PersonActivity.Eating},
      {"walk", PersonActivity.Walking},
      {"run", PersonActivity.Running},
      {"nap", PersonActivity.Napping},
      {"god mode", PersonActivity.GodMode},
    };
  }

  public class Experimentor
  {
    public Person Person;
    public ExperimentorConfig Config;
    public Logger MyLogger;

    public Experimentor() : this(new ExperimentorConfig())
    {

    }

    public Experimentor(ExperimentorConfig config)
    {
      MyLogger = new Logger(config.DebuggerOn);
      Person = new Person(config.DefaultPersonActivity, config.DefaultPersonState);
      Config = config;

      MyLogger.Debug($"Experimentor - Person starting state is...\n{Person.State}");
    }

    public void CheckState()
    {
      StateFactors currentStateFactors = GetFactorsForPersonActivity();
      MyLogger.Debug(currentStateFactors.ToString());
    }

    public StateFactors GetFactorsForPersonActivity()
    {
      switch (Person.Activity)
      {
        case PersonActivity.Idle:
          return Config.IdleFactors;
        case PersonActivity.Eating:
          return Config.EatingFactors;
        case PersonActivity.Drinking:
          return Config.DrinkingFactors;
        case PersonActivity.Napping:
          return Config.NappingFactors;
        case PersonActivity.Sleeping:
          return Config.SleepingFactors;
        case PersonActivity.Walking:
          return Config.WalkingFactors;
        case PersonActivity.Running:
          return Config.RunningFactors;
        default:
          throw new Exception($"{Person.Activity} is not a supported state");
      }
    }

    public void ApplyStateToPerson()
    {
      StateFactors currentStateFactors = GetFactorsForPersonActivity();

      MyLogger.Debug("\n\nApplying state...");
      MyLogger.Debug($"Current Factors:  {currentStateFactors}");
      MyLogger.Debug($"{Person}");

      Person.State.Satiation += currentStateFactors.Satiation;
      Person.State.Hydration += currentStateFactors.Hydration;
      Person.State.Energy += currentStateFactors.Energy;
      Person.State.Weight += currentStateFactors.Weight;
    }

    public void UpdatePersonActivity(string text)
    {
      PersonActivity activity;
      if (!Config.AcivityMap.TryGetValue(text, out activity))
      {
        throw new UnsupportedActivityException($"`{text}` is an unsupported activity.");
      }

      if (activity == PersonActivity.GodMode)
      {
        throw new EnlightenmentAttainedException();
      }

      Person.Activity = activity;
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

  public class UnsupportedActivityException : Exception
  {
    public UnsupportedActivityException(string message)
            : base(message)
    {
    }
  }

  public class EnlightenmentAttainedException : Exception { }
}