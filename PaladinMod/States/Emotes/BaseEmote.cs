﻿using EntityStates;
using PaladinMod.Misc;
using RoR2;
using UnityEngine;

namespace PaladinMod.States.Emotes
{
    public class BaseEmote : BaseState
    {
        public string soundString;
        public string animString;
        public float duration;
        public float animDuration;
        public bool normalizeModel;

        private uint activePlayID;
        private Animator animator;
        protected ChildLocator childLocator;
        //private CharacterCameraParams originalCameraParams;
        protected PaladinSwordController swordController;
        public LocalUser localUser;
        private CameraTargetParams.CameraParamsOverrideHandle handle;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();
            this.swordController = base.GetComponent<PaladinSwordController>();
            this.localUser = LocalUserManager.readOnlyLocalUsersList[0];
            
            base.characterBody.hideCrosshair = true;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = false;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);

            if (this.animDuration == 0 && this.duration != 0) this.animDuration = this.duration;

            if (this.duration > 0) base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.duration);
            else base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.animDuration);

            this.activePlayID = Util.PlaySound(soundString, base.gameObject);

            if (this.normalizeModel)
            {
                if (base.modelLocator)
                {
                    base.modelLocator.normalizeToFloor = true;
                }
            }

            //this.originalCameraParams = base.cameraTargetParams.cameraParams;
            handle = Modules.CameraParams.OverridePaladinCameraParams(base.cameraTargetParams, PaladinCameraParams.EMOTE, 0.5f);
        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterBody.hideCrosshair = false;
            //base.cameraTargetParams.cameraParams = this.originalCameraParams;
            //base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Standard);
            //Modules.CameraParams.OverridePaladinCameraParams(base.cameraTargetParams, PaladinCameraParams.DEFAULT);
            base.cameraTargetParams.RemoveParamsOverride(handle, 0.2f);

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = true;
            if (this.animator)
            {
                this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
                this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);
            }

            if (this.normalizeModel)
            {
                if (base.modelLocator)
                {
                    base.modelLocator.normalizeToFloor = false;
                }
            }

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (this.activePlayID != 0) AkSoundEngine.StopPlayingID(this.activePlayID);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool flag = false;

            if (base.characterMotor)
            {
                if (!base.characterMotor.isGrounded) flag = true;
                if (base.characterMotor.velocity != Vector3.zero) flag = true;
            }

            if (base.inputBank)
            {
                if (base.inputBank.skill1.down) flag = true;
                if (base.inputBank.skill2.down) flag = true;
                if (base.inputBank.skill3.down) flag = true;
                if (base.inputBank.skill4.down) flag = true;
                if (base.inputBank.jump.down) flag = true;

                if (base.inputBank.moveVector != Vector3.zero) flag = true;
            }

            //emote cancels
            if (base.isAuthority && base.characterMotor.isGrounded && !this.localUser.isUIFocused)
            {
                if (Input.GetKeyDown(Modules.Config.praiseKeybind.Value))
                {
                    this.outer.SetInterruptState(new PraiseTheSun(), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Modules.Config.restKeybind.Value))
                {
                    this.outer.SetInterruptState(new Rest(), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Modules.Config.pointKeybind.Value))
                {
                    this.outer.SetInterruptState(new PointDown(), InterruptPriority.Any);
                    return;
                } else if (Input.GetKeyDown(Modules.Config.swordPoseKeybind.Value)) {
                    this.outer.SetInterruptState(new TestPose(), InterruptPriority.Any);
                    return;
                }
            }

            if (this.duration > 0 && base.fixedAge >= this.duration) flag = true;

            if (this.animator) this.animator.SetBool("inCombat", true);

            if (flag)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }

}