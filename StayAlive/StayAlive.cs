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
        // UnityEngine.Debug.Log(message); // uncomment this for unity
        Console.WriteLine(message); // comment this for unity
      }
    }

    public void DebugDictionary<K, V>(Dictionary<K, V> dict)
    {
      Debug(string.Join(Environment.NewLine, dict));
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
        _satiation = CheckedNewValue(value);
        CheckFactorSet(value, new StarvedException(), new StuffedException());
      }
    }

    private float _hydration;
    public float Hydration
    {
      get => _hydration;
      set
      {
        _hydration = CheckedNewValue(value);
        CheckFactorSet(value, new ParchedException(), new SaturatedException());
      }
    }

    private float _energy;
    public float Energy
    {
      get => _energy;
      set
      {
        _energy = CheckedNewValue(value);
        CheckFactorSet(value, new DrainedException(), new CrankedException());
      }
    }

    private float _weight;
    public float Weight
    {
      get => _weight;
      set
      {
        _weight = CheckedNewValue(value);
        CheckFactorSet(value, new UnderweightException(), new OverweightException());
      }
    }

    public StateFactors(float satiation, float hydration, float energy, float weight)
    {
      _satiation = satiation;
      _hydration = hydration;
      _energy = energy;
      _weight = weight;
    }

    private static float CheckedNewValue(float newValue)
    {
      return newValue >= 0 && newValue <= 100 ? newValue : 0;
    }

    private static void CheckFactorSet(float newValue, Exception lowException, Exception highException)
    {
      if (newValue >= 100f)
      {
        throw highException;
      }

      if (newValue <= 0f)
      {
        throw lowException;
      }
    }

    public override string ToString()
    {
      return $"Satiation: {_satiation}\tHydration: {_hydration}\tStamina: {_energy}\tWeight: {_weight}";
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

  public enum PersonCondition
  {
    Starved,
    Stuffed,
    Parched,
    Saturated,
    Drained,
    Cranked,
    Underweight,
    Overweight
  }

  public class Person
  {
    public PersonActivity Activity;
    public StateFactors State;
    public Dictionary<PersonCondition, bool> ConditionFlags = new Dictionary<PersonCondition, bool>();

    public Person(PersonActivity activity, StateFactors state)
    {
      Activity = activity;
      State = state;

      foreach (var condition in (PersonCondition[])Enum.GetValues(typeof(PersonCondition)))
      {
        ConditionFlags[condition] = false;
      }
    }

    override public string ToString()
    {
      return $"Activity: {Activity}\t{State}";
    }
  }

  public class ExperimentorConfig
  {
    public bool DebuggerOn = true;
    public PersonActivity DefaultPersonActivity = PersonActivity.Idle;
    public StateFactors DefaultPersonState = new StateFactors(50, 50, 50, 59);
    public StateFactors StateFactorMaximums = new StateFactors(100, 100, 100, 100);

    #region Activity Factors
    public StateFactors IdleFactors = new StateFactors(-1, -1, 0, -0.5f);
    public StateFactors EatingFactors = new StateFactors(+1, -1, 0, +0.5f);
    public StateFactors DrinkingFactors = new StateFactors(-1, 1, 0, -0.5f);
    public StateFactors NappingFactors = new StateFactors(-1, -1, +0.5f, -0.35f);
    public StateFactors SleepingFactors = new StateFactors(-1, -1, +1, -0.25f);
    public StateFactors WalkingFactors = new StateFactors(-2.5f, -1.5f, -2, -1);
    public StateFactors RunningFactors = new StateFactors(-5, -3, -4, -2);
    #endregion Activity Factors

    #region Edge Case Configs
    public float PenaltyMultiplier = 1.25f;
    public float StarvedPenalty;
    public float StuffedPenalty;
    public float ParchedPenalty;
    public float SaturatedPenalty;
    public float DrainedPenalty;
    public float CrankedPenalty;
    #endregion Edge Case Configs
    public Dictionary<string, PersonActivity> AcivityMap = new Dictionary<string, PersonActivity>(){
      {"relax", PersonActivity.Idle},
      {"drink", PersonActivity.Drinking},
      {"eat", PersonActivity.Eating},
      {"walk", PersonActivity.Walking},
      {"run", PersonActivity.Running},
      {"nap", PersonActivity.Napping},
      {"god mode", PersonActivity.GodMode},
    };

    public ExperimentorConfig()
    {
      StarvedPenalty = -1f * PenaltyMultiplier;
      StuffedPenalty = PenaltyMultiplier;
      ParchedPenalty = -1f * PenaltyMultiplier;
      SaturatedPenalty = PenaltyMultiplier;
      DrainedPenalty = -1f * PenaltyMultiplier;
      CrankedPenalty = PenaltyMultiplier;
    }
  }

  public class Experimentor
  {
    public readonly ExperimentorConfig Config;
    public readonly Person Person;
    private Logger MyLogger;

    public Experimentor() : this(new ExperimentorConfig())
    {

    }

    public Experimentor(ExperimentorConfig config)
    {
      Person = new Person(config.DefaultPersonActivity, config.DefaultPersonState);
      Config = config;

      Initialize();
    }

    public Experimentor(ExperimentorConfig config, Person person)
    {
      Person = person;
      Config = config;

      Initialize();
    }

    private void Initialize()
    {
      MyLogger = new Logger(Config.DebuggerOn);

      MyLogger.Debug($"Experimentor - Person starting as...\n{Person}\n");
    }

    public void CheckState()
    {
      StateFactors currentStateFactors = GetFactorsForPersonActivity();
      MyLogger.Debug(currentStateFactors.ToString());
    }

    public StateFactors GetFactorsForPersonActivity()
    {
      return Person.Activity switch
      {
        PersonActivity.Idle => Config.IdleFactors,
        PersonActivity.Eating => Config.EatingFactors,
        PersonActivity.Drinking => Config.DrinkingFactors,
        PersonActivity.Napping => Config.NappingFactors,
        PersonActivity.Sleeping => Config.SleepingFactors,
        PersonActivity.Walking => Config.WalkingFactors,
        PersonActivity.Running => Config.RunningFactors,
        _ => throw new Exception($"{Person.Activity} is not a supported state"),
      };
    }

    public void ApplyStateToPerson()
    {
      StateFactors currentStateFactors = GetFactorsForPersonActivity();

      MyLogger.Debug("\n\nApplying state...");
      MyLogger.Debug($"Current Factors:  {currentStateFactors}");
      MyLogger.Debug($"{Person}");
      MyLogger.DebugDictionary(Person.ConditionFlags);

      Dictionary<PersonCondition, bool> newConditionFlags = new Dictionary<PersonCondition, bool>(Person.ConditionFlags);

      try
      {
        float satiationPenalty = Person.ConditionFlags[PersonCondition.Cranked] ? Config.CrankedPenalty : 0f;
        Person.State.Satiation += currentStateFactors.Satiation + satiationPenalty;
        newConditionFlags[PersonCondition.Starved] = false;
        newConditionFlags[PersonCondition.Stuffed] = false;
      }
      catch (StarvedException)
      {
        newConditionFlags[PersonCondition.Starved] = true;
      }
      catch (StuffedException)
      {
        newConditionFlags[PersonCondition.Stuffed] = true;
      }

      try
      {
        Person.State.Hydration += currentStateFactors.Hydration;
        newConditionFlags[PersonCondition.Parched] = false;
        newConditionFlags[PersonCondition.Saturated] = false;
      }
      catch (ParchedException)
      {
        newConditionFlags[PersonCondition.Parched] = true;
      }
      catch (SaturatedException)
      {
        newConditionFlags[PersonCondition.Saturated] = true;
      }

      try
      {
        float parchedPenalty = Person.ConditionFlags[PersonCondition.Parched] ? Config.ParchedPenalty : 0;
        float saturatedPenalty = Person.ConditionFlags[PersonCondition.Saturated] ? Config.SaturatedPenalty : 0;
        Person.State.Energy += currentStateFactors.Energy + parchedPenalty + saturatedPenalty;
        newConditionFlags[PersonCondition.Drained] = false;
        newConditionFlags[PersonCondition.Cranked] = false;
      }
      catch (DrainedException)
      {
        newConditionFlags[PersonCondition.Drained] = true;
      }
      catch (CrankedException)
      {
        newConditionFlags[PersonCondition.Cranked] = true;
      }

      float starvedPenalty = Person.ConditionFlags[PersonCondition.Starved] ? Config.StarvedPenalty : 0;
      float stuffedPenalty = Person.ConditionFlags[PersonCondition.Stuffed] ? Config.StuffedPenalty : 0;
      float drainedPenalty = Person.ConditionFlags[PersonCondition.Drained] ? Config.DrainedPenalty : 0;

      // Let Overweight and Underweight exceptions go through
      Person.State.Weight += currentStateFactors.Weight + starvedPenalty + stuffedPenalty + drainedPenalty;

      Person.ConditionFlags = newConditionFlags;

      MyLogger.Debug("Post state application...");
      MyLogger.Debug($"{Person}");
      MyLogger.DebugDictionary(newConditionFlags);
    }

    public void UpdatePersonActivity(string text)
    {
      if (!Config.AcivityMap.TryGetValue(text, out PersonActivity activity))
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