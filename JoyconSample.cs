using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using VRM;

public class JoyConSample : MonoBehaviour
    
{
    [SerializeField] private GameObject playerArmature;
    [SerializeField] private CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] private GameObject VRMmodel;
    [SerializeField] private GameObject externalReceiver;

    [SerializeField] private float cameraSpeedHorizontal = 0.2f;
    [SerializeField] private float cameraSpeedHeight = -0.11f;
    [SerializeField] private float cameraSpeedDistance = 0.005f;

    public BlendShapePreset presetX = BlendShapePreset.Neutral;
    public BlendShapePreset presetY = BlendShapePreset.Sorrow;
    public BlendShapePreset presetA = BlendShapePreset.Joy;
    public BlendShapePreset presetB = BlendShapePreset.Angry;

    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private VRMBlendShapeProxy blendShapeProxy;
    private Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;

    public enum ButtonName : int
    {
        DPAD_DOWN = 0,
        DPAD_RIGHT = 1,
        DPAD_LEFT = 2,
        DPAD_UP = 3,
        SL = 4,
        SR = 5,
        MINUS = 6,
        HOME = 7,
        PLUS = 8,
        CAPTURE = 9,
        STICK = 10,
        SHOULDER_1 = 11,
        SHOULDER_2 = 12
    };

    private bool[] isButtonL = new bool[13];
    private bool[] isButtonR = new bool[13];

    private float[] stickL;
    private float[] stickR;

    private void Start()
    {
        SetControllers();

        starterAssetsInputs = playerArmature.GetComponent<StarterAssetsInputs>();
        animator = playerArmature.GetComponent<Animator>();
        blendShapeProxy = VRMmodel.GetComponent<VRMBlendShapeProxy>();

        animator.enabled = false;
        externalReceiver.SetActive(true);

        cinemachine3RdPersonFollow = playerFollowCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        cinemachine3RdPersonFollow.ShoulderOffset.x = 0f;
    }

    private void Update()
    {

        if (m_joycons == null || m_joycons.Count <= 0) return;

        SetControllers();

        stickL = m_joyconL.GetStick();
        stickR = m_joyconR.GetStick();


        if (stickL != null)
        {
            if (stickL[0]!=0 || stickL[1]!=0)
            {
                if (!isButtonL[(int)ButtonName.DPAD_UP])
                {
                    changeMotion();
                    buttonStateChange(true, (int)ButtonName.DPAD_UP, true);
                }
            }
            else
            {
                if (isButtonL[(int)ButtonName.DPAD_UP])
                {
                    changeMotion();
                    buttonStateChange(true, (int)ButtonName.DPAD_UP, false);
                }
            }

            starterAssetsInputs.MoveInput(new Vector2(stickL[0], stickL[1]));
        }


        if(stickR != null)
        {
            if (!isButtonR[11])
            {
                starterAssetsInputs.LookInput(new Vector2(stickR[0] * cameraSpeedHorizontal, stickR[1] * cameraSpeedHeight));
            }
        }
    }

    private void LateUpdate()
    {
        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var button in m_buttons)
        {
            //左コントローラ
            if (m_joyconL.GetButtonDown(button))//押したとき
            {
                if (!isButtonL[button.GetHashCode()])
                {
                    switch (button.GetHashCode())
                    {
                        case (int)ButtonName.DPAD_UP:
                            changeMotion();
                            break;
                        case (int)ButtonName.SHOULDER_2:
                            starterAssetsInputs.SprintInput(true);
                            break;
                        default:
                            break;
                    }
                }
                buttonStateChange (true,button.GetHashCode(),true);

            }

            if (m_joyconL.GetButton(button))//押し中
            {
                switch (button.GetHashCode())
                {
                    default:
                        break;
                }
            }
            if (m_joyconL.GetButtonUp(button))//離したとき
            {
                if (isButtonL[button.GetHashCode()])
                {
                    switch (button.GetHashCode())
                    {
                        case (int)ButtonName.SHOULDER_2:
                            starterAssetsInputs.SprintInput(false);
                            break;
                        default:
                            break;
                    }
                }
                buttonStateChange(true,button.GetHashCode(),false);
            }

            //右コントローラ
            if (m_joyconR.GetButtonDown(button))//押したとき
            {
                if (!isButtonR[button.GetHashCode()])
                {
                    switch (button.GetHashCode())
                    {
                        case (int)ButtonName.SHOULDER_1:
                            if (stickR != null)
                            {
                                cinemachine3RdPersonFollow.CameraDistance += stickR[1]*cameraSpeedDistance;
                            }
                            break;
                        case (int)ButtonName.SHOULDER_2:
                            starterAssetsInputs.JumpInput(true);
                            break;
                        case (int)ButtonName.DPAD_UP:
                            foreach (BlendShapePreset preset in Enum.GetValues(typeof(BlendShapePreset)))
                            {
                                blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(preset), 0f);
                            }
                            blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(presetX), 1f);
                            break;
                        case (int)ButtonName.DPAD_RIGHT:
                            foreach (BlendShapePreset preset in Enum.GetValues(typeof(BlendShapePreset)))
                            {
                                blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(preset), 0f);
                            }
                            blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(presetA), 1f);
                            break;
                        case (int)ButtonName.DPAD_LEFT:
                            foreach (BlendShapePreset preset in Enum.GetValues(typeof(BlendShapePreset)))
                            {
                                blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(preset), 0f);
                            }
                            blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(presetY), 1f);
                            break;
                        case (int)ButtonName.DPAD_DOWN:
                            foreach (BlendShapePreset preset in Enum.GetValues(typeof(BlendShapePreset)))
                            {
                                blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(preset), 0f);
                            }
                            blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(presetB), 1f);
                            break;
                        default:
                            break;
                    }
                }
                buttonStateChange(false,button.GetHashCode(),true);

            }

            if (m_joyconR.GetButton(button))//押し中
            {
                switch (button.GetHashCode())
                {
                    case(int)ButtonName.SHOULDER_1:
                        if(stickR != null)
                        {
                            cinemachine3RdPersonFollow.CameraDistance += stickR[1]*cameraSpeedDistance;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (m_joyconR.GetButtonUp(button))//離したとき
            {
                if (isButtonR[button.GetHashCode()])
                {
                    switch (button.GetHashCode())
                    {
                        default:
                            break;
                    }
                }
                buttonStateChange(false,button.GetHashCode(),false);
            }
        }
    }


    private void SetControllers()
    {
        m_joycons = JoyconManager.Instance.j;
        if (m_joycons == null || m_joycons.Count <= 0) return;
        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }


    public void buttonStateChange(bool isLeft, int number ,bool state)
    {
        if (isLeft)
        {
            isButtonL[number] = state;
        }
        else
        {
            isButtonR[number] = state;
        }
    }

    public void changeMotion()
    {
        if (animator.enabled == true)
        {
            animator.enabled = false;
            externalReceiver.SetActive(true);
        }
        else
        {
            animator.enabled = true;
            externalReceiver.SetActive(false);
        }
    }
}
