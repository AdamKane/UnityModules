using UnityEngine.Playables;

namespace Leap.Unity.Recording {

  public class RecordingMixerBehaviour : PlayableBehaviour {

    private Frame _frame;

    public override void OnGraphStart(Playable playable) {
      base.OnGraphStart(playable);

      _frame = new Frame();
    }

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
      LeapPlayableProvider provider = playerData as LeapPlayableProvider;

      if (!provider)
        return;

      int inputCount = playable.GetInputCount();

      for (int i = 0; i < inputCount; i++) {
        float inputWeight = playable.GetInputWeight(i);
        var inputPlayable = (ScriptPlayable<RecordingBehaviour>)playable.GetInput(i);
        var input = inputPlayable.GetBehaviour();

        if (inputWeight > 0 && input.recording != null) {
          double percent = input.recording.length * inputPlayable.GetTime() / inputPlayable.GetDuration();
          if (input.recording.Sample((float)percent, _frame, clampTimeToValid: true)) {
            provider.SetCurrentFrame(_frame);
            break;
          }
        }
      }
    }
  }
}
