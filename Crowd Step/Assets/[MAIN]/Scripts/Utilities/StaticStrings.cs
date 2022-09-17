using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdStep
{
    public static class StaticStrings
    {
        //Gameobject Tags
        public const string Character_Tag = "Character";

        public const string Floor_Tag = "Floor";

        public const string Diamond_Tag = "Diamond";

        public const string FinishStep_Tag = "FinishStep";

        public const string Obstacle_Tag = "Obstacle";

        //Anim Params Tags
        public static int Move_AnimParam = Animator.StringToHash("Move");
        public static int Win_AnimParam = Animator.StringToHash("OnWin");
        public static int Lose_AnimParam = Animator.StringToHash("OnLose");
        public static int Cry_AnimParam = Animator.StringToHash("Cry");

        //Input Params
        public static string PlayerWin_DoozyEvent = "Player Win";
        public static string PlayerLose_DoozyEvent = "Player Lose";


        //Input Params
        public static string Ascend_Input = "Ascend";
    }
}
