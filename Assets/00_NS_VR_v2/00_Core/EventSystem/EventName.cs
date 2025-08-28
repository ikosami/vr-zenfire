using System;
using System.Collections.Generic;
using System.Data;

namespace Ev.Events
{
    public static class Name
    {
        public const string POPUP_OPEN = "POPUP_OPEN";
        public const string POPUP_CLOSE = "POPUP_CLOSE";
        public const string HAND_POSE_CHANGE = "HAND_POSE_CHANGE";
        public const string HAND_POSE_RESET = "HAND_POSE_RESET";
        public const string HAND_POSE_LOCK = "HAND_POSE_LOCK";
        public const string HAND_POSE_UNLOCK = "HAND_POSE_UNLOCK";
    }

    public class DefaultEvent : IEvent
    {
        public string EventName { get => _eventName; }

        string _eventName;
        public DefaultEvent(string Key)
        {
            _eventName = Key;
        }

        public Dictionary<string, object> ToDic()
        {
            return new Dictionary<string, object>();
        }
    }

    public class PopUpOpen : IEvent
    {
        public const string POPUP_NAME = "popupName";

        public string EventName => Name.POPUP_OPEN;

        public string PopUpName { get; set; }

        public Dictionary<string, object> ToDic()
        {
            return new Dictionary<string, object>() { { POPUP_NAME, PopUpName } };
        }
    }

    public class PopUpClose : IEvent
    {
        public const string POPUP_NAME = "popupName";

        public string EventName => Name.POPUP_CLOSE;

        public string PopUpName { get; set; }

        public Dictionary<string, object> ToDic()
        {
            return new Dictionary<string, object>() { { POPUP_NAME, PopUpName } };
        }
    }

    public class HandPoseChange : IEvent
    {
        public const string POSE_NAME = "poseName";
        public const string CONTROLLER_SIDE = "controllerSide";
        public const string HAND_PART_TYPE = "handPartType";

        public string EventName => Name.HAND_POSE_CHANGE;
        public HandPartType HandPartType { get; set; }
        public string PoseName { get; set; }
        public ControllerSide ControllerSide { get; set; }
        public Dictionary<string, object> ToDic()
        {
            return new Dictionary<string, object>() { { POSE_NAME, PoseName }, { CONTROLLER_SIDE, ControllerSide }, { HAND_PART_TYPE, HandPartType } };
        }
    }

    public class HandPoseLock : IEvent
    {
        public const string CONTROLLER_SIDE = "controllerSide";
        public const string HAND_PART_TYPE = "handPartType";

        public string EventName => Name.HAND_POSE_LOCK;
        public ControllerSide ControllerSide { get; set; }
        public HandPartType HandPartType { get; set; }
        public Dictionary<string, object> ToDic() {
            return new Dictionary<string, object>(){{CONTROLLER_SIDE, ControllerSide}, {HAND_PART_TYPE, HandPartType}};
        }
    }

    public class HandPoseUnlock : IEvent
    {
        public const string CONTROLLER_SIDE = "controllerSide";
        public const string HAND_PART_TYPE = "handPartType";
        public string EventName => Name.HAND_POSE_UNLOCK;
        public ControllerSide ControllerSide { get; set; }
        public HandPartType HandPartType { get; set; }
        public Dictionary<string, object> ToDic() {
            return new Dictionary<string, object>(){{CONTROLLER_SIDE, ControllerSide}, {HAND_PART_TYPE, HandPartType}};
        }
    }

    public class HandPoseReset : IEvent
    {
        public const string CONTROLLER_SIDE = "controllerSide";
        public const string HAND_PART_TYPE = "handPartType";
        public string EventName => Name.HAND_POSE_RESET;
        public ControllerSide ControllerSide;
        public HandPartType HandPartType;
        public Dictionary<string, object> ToDic() {
            return new Dictionary<string, object>(){{CONTROLLER_SIDE, ControllerSide}, {HAND_PART_TYPE, HandPartType}};
        }
    }
}