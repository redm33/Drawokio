using UnityEngine;
using System.Collections;

public class AchievementTrigger : Resettable {
    public string objectTitle;
    public FadingText text;

    public override void PerformReset() {
        pickedUp = false;
    }
    public override bool isProgress {
        get {
            return true;
        }
    }

}
