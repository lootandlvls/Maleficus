using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

/// <summary>
/// Contains utility functions.
/// </summary>
public static class MaleficusUtilities
{

    #region General
    /// <summary> Gets a random index for a given array </summary>
    public static int GetRndIndex(int arrayLength)
    {
        return Random.Range(0, arrayLength);
    }

    /// <summary> Gets a random rotation over Y axis. Can be used to get a random orientation for a gived character. </summary>
    public static Quaternion GetRndStandRotation()
    {
        return Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
    }

    public static void TransformAxisToCamera(ref float axis_X, ref float axis_Z, Vector3 cameraForwardDirection, bool isRotation = false)
    {
        Vector2 coordinateForward = new Vector2(0.0f, 1.0f);
        Vector2 coordinateRight = new Vector2(1.0f, 0.0f);
        Vector2 cameraForward = new Vector2(cameraForwardDirection.normalized.x, cameraForwardDirection.normalized.z).normalized;
        Vector2 controllerAxis = new Vector2(axis_X, axis_Z).normalized;
        float dotWithRight = Vector2.Dot(coordinateRight, cameraForward);
        int sign;
        if (dotWithRight > 0.0f)
        {
            sign = -1;
        }
        else if (dotWithRight < 0.0f)
        {
            sign = 1;
        }
        else
        {
            sign = 0;
        }
        if (isRotation == true)
        {
            sign *= -1;
        }

        float angle = Mathf.Acos(Vector2.Dot(coordinateForward, cameraForward)) * sign;
        DebugManager.Instance.Log(68, "X : " + controllerAxis.x + " | Y : " + controllerAxis.y + " | A : " + angle * Mathf.Rad2Deg);


        axis_Z = controllerAxis.y * Mathf.Cos(angle) + controllerAxis.x * Mathf.Sin(angle);
        axis_X = controllerAxis.x * Mathf.Cos(angle) - controllerAxis.y * Mathf.Sin(angle);
        controllerAxis = new Vector2(axis_X, axis_Z).normalized;

        axis_X = controllerAxis.x;
        axis_Z = controllerAxis.y;
    }
    #endregion

    #region Sound
    /// <summary>
    /// Plays a random clip from a given clip soundbank on a given AudioSource component
    /// </summary>
    public static void PlayRandomSound(AudioSource source, AudioClip[] clips)
    {
        if (clips.Length != 0)
        {
            if (source != null)
            {
                source.clip = clips[GetRndIndex(clips.Length)];
                source.Play();
            }
            else
            {
                Debug.LogWarning("No AudioSource attached!");
            }

        }
        else
        {
            Debug.LogWarning("No audio clip attached!");
        }
    }

    public static void PlaySound(AudioSource source, AudioClip clip)
    {
        if (source != null)
        {
            source.clip = clip;
            source.Play();
        }
        else
        {
            Debug.LogWarning("No AudioSource attached!");
        }
    }

    public static void StopSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
        }
        else
        {
            Debug.LogWarning("No AudioSource attached!");
        }
    }


    /* Audio Listeners */
    public static AudioSource AddAudioListener(GameObject toGameObject)
    {
        AudioSource aS = toGameObject.AddComponent<AudioSource>();
        aS.playOnAwake = false;

        return aS;
    }

    public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D)
    {
        AudioSource aS = toGameObject.AddComponent<AudioSource>();
        aS.playOnAwake = false;

        if (is3D == true)
        {
            aS.spatialBlend = 1.0f;
        }
        else
        {
            aS.spatialBlend = 0.0f;
        }
        return aS;
    }

    public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D, float volume)
    {
        AudioSource aS = toGameObject.AddComponent<AudioSource>();
        aS.playOnAwake = false;

        if (is3D == true)
        {
            aS.spatialBlend = 1.0f;
        }
        else
        {
            aS.spatialBlend = 0.0f;
        }
        aS.volume = volume;
        return aS;
    }

    public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D, float volume, bool isLoop)
    {
        AudioSource aS = toGameObject.AddComponent<AudioSource>();
        aS.playOnAwake = false;

        if (is3D == true)
        {
            aS.spatialBlend = 1.0f;
        }
        else
        {
            aS.spatialBlend = 0.0f;
        }
        aS.volume = volume;
        aS.loop = isLoop;
        return aS;
    }

    public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D, float volume, bool isLoop, AudioMixerGroup audioMixerGroup)
    {
        AudioSource aS = toGameObject.AddComponent<AudioSource>();
        aS.playOnAwake = false;
        aS.outputAudioMixerGroup = audioMixerGroup;
        if (is3D == true)
        {
            aS.spatialBlend = 1.0f;
        }
        else
        {
            aS.spatialBlend = 0.0f;
        }
        aS.volume = volume;
        aS.loop = isLoop;
        return aS;
    }
    #endregion

    #region Maleficus Conversions
    /// <summary> Convert a PlayerID enum to an int </summary>
    public static int PlayerIDToInt(EPlayerID playerID)
    {
        int id = 0;
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                id = 1;
                break;
            case EPlayerID.PLAYER_2:
                id = 2;
                break;
            case EPlayerID.PLAYER_3:
                id = 3;
                break;
            case EPlayerID.PLAYER_4:
                id = 4;
                break;
        }
        return id;
    }

    /// <summary> Convert an int to a PlayerID enum </summary>
    public static EPlayerID IntToPlayerID(int playerID)
    {
        EPlayerID id = EPlayerID.NONE;
        switch (playerID)
        {
            case 1:
                id = EPlayerID.PLAYER_1;
                break;
            case 2:
                id = EPlayerID.PLAYER_2;
                break;
            case 3:
                id = EPlayerID.PLAYER_3;
                break;
            case 4:
                id = EPlayerID.PLAYER_4;
                break;
        }
        return id;
    }       
    /// <summary> Convert an int to a ClientID enum </summary>
    public static EClientID IntToClientID(int clientID)
    {
        EClientID id = EClientID.NONE;
        switch (clientID)
        {
            case 0:
                id = EClientID.SERVER;
                break;
            case 1:
                id = EClientID.CLIENT_1;
                break;
            case 2:
                id = EClientID.CLIENT_2;
                break;
            case 3:
                id = EClientID.CLIENT_3;
                break;
            case 4:
                id = EClientID.CLIENT_4;
                break;
        }
        return id;
    }       
    
    /// <summary> Convert an int to a SpellID enum </summary>
    public static ESpellID IntToSpellID(int spellID)
    {
        ESpellID id = ESpellID.NONE;
        switch (spellID)
        {
            case 1:
                id = ESpellID.SPELL_1;
                break;
            case 2:
                id = ESpellID.SPELL_2;
                break;
            case 3:
                id = ESpellID.SPELL_3;
                break;
        }
        return id;
    }    
    
    /// <summary> Convert a ControllerID enum to an Char </summary>
    public static char ControllerIDToChar(EControllerID ControllerID)
    {
        char id = 'X';
        switch (ControllerID)
        {
            case EControllerID.CONTROLLER_A:
                id = 'A';
                break;
            case EControllerID.CONTROLLER_B:
                id = 'B';
                break;
            case EControllerID.CONTROLLER_C:
                id = 'C';
                break;
            case EControllerID.CONTROLLER_D:
                id = 'D';
                break;
        }
        return id;
    }

    /// <summary> Convert an Char to a ControllerID enum </summary>
    public static EControllerID CharToControllerID(char ControllerID)
    {
        EControllerID id = EControllerID.NONE;
        switch (ControllerID)
        {
            case 'A':
                id = EControllerID.CONTROLLER_A;
                break;
            case 'B':
                id = EControllerID.CONTROLLER_B;
                break;
            case 'C':
                id = EControllerID.CONTROLLER_C;
                break;
            case 'D':
                id = EControllerID.CONTROLLER_D;
                break;
        }
        return id;
    }

    /// <summary> Convert an InputButton enum to a SpellID enum </summary>
    public static ESpellID GetSpellIDFrom(EInputButton inputButton)
    {
        ESpellID id = ESpellID.NONE;
        switch (inputButton)
        {
            case EInputButton.CAST_SPELL_1:
                id = ESpellID.SPELL_1;
                break;

            case EInputButton.CAST_SPELL_2:
                id = ESpellID.SPELL_2;
                break;

            case EInputButton.CAST_SPELL_3:
                id = ESpellID.SPELL_3;
                break;
        }
        return id;
    }

    /// <summary> Convert a TouchJoystickType enum to a SpellID enum </summary>
    public static ESpellID GetSpellIDFrom(ETouchJoystickType inputButton)
    {
        ESpellID id = ESpellID.NONE;
        switch (inputButton)
        {
            case ETouchJoystickType.SPELL_1:
                id = ESpellID.SPELL_1;
                break;

            case ETouchJoystickType.SPELL_2:
                id = ESpellID.SPELL_2;
                break;

            case ETouchJoystickType.SPELL_3:
                id = ESpellID.SPELL_3;
                break;
        }
        return id;
    }

    /// <summary> Convert a TouchJoystickType enum to a InputButton enum </summary>
    public static EInputButton GetInputButtonFrom(ETouchJoystickType touchJoystickType)
    {
        EInputButton button = EInputButton.NONE;
        switch (touchJoystickType)
        {
            case ETouchJoystickType.SPELL_1:
                button = EInputButton.CAST_SPELL_1;
                break;

            case ETouchJoystickType.SPELL_2:
                button = EInputButton.CAST_SPELL_2;
                break;

            case ETouchJoystickType.SPELL_3:
                button = EInputButton.CAST_SPELL_3;
                break;
        }
        return button;
    }

    /// <summary> Convert a SpellID enum to a InputButton enum </summary>
    public static EInputButton GetInputButtonFrom(ESpellID spellID)
    {
        EInputButton button = EInputButton.NONE;
        switch (spellID)
        {
            case ESpellID.SPELL_1:
                button = EInputButton.CAST_SPELL_1;
                break;

            case ESpellID.SPELL_2:
                button = EInputButton.CAST_SPELL_2;
                break;

            case ESpellID.SPELL_3:
                button = EInputButton.CAST_SPELL_3;
                break;
        }
        return button;
    }

    /// <summary> Convert a ClientID enum to a PlayerID enum </summary>
    public static EPlayerID GetPlayerIDFrom(EClientID clientID)
    {
        EPlayerID id = EPlayerID.NONE;
        switch (clientID)
        {
            case EClientID.CLIENT_1:
                id = EPlayerID.PLAYER_1;
                break;

            case EClientID.CLIENT_2:
                id = EPlayerID.PLAYER_2;
                break;

            case EClientID.CLIENT_3:
                id = EPlayerID.PLAYER_3;
                break;

            case EClientID.CLIENT_4:
                id = EPlayerID.PLAYER_4;
                break;
        }
        return id;
    }

    /// <summary> Returns the same team ID as the player ID (e.g. Player 2 -> Team 2)</summary>
    public static ETeamID GetIdenticPlayerTeam(EPlayerID playerID)
    {
        ETeamID id = ETeamID.NONE;
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                id = ETeamID.TEAM_1;
                break;

            case EPlayerID.PLAYER_2:
                id = ETeamID.TEAM_2;
                break;

            case EPlayerID.PLAYER_3:
                id = ETeamID.TEAM_3;
                break;

            case EPlayerID.PLAYER_4:
                id = ETeamID.TEAM_4;
                break;
        }
        return id;
    }

    #endregion
}
