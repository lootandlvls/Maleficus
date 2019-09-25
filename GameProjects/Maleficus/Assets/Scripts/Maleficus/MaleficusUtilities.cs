using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Contains utility functions.
/// </summary>
public static class MaleficusUtilities
{

    #region General
    /// <summary> Gets a random index for a given array </summary>
    public static int GetRndIndex(int arrayLength)
    {
        return UnityEngine.Random.Range(0, arrayLength);
    }

    /// <summary> Gets a random rotation over Y axis. Can be used to get a random orientation for a gived character. </summary>
    public static Quaternion GetRndStandRotation()
    {
        return Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
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

    #region Networking
    private const string EMAIL_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
    private const string USERNAME_AND_DISCRIMINATOR_PATTERN = @"^[a-zA-Z0-9]{4,20}#[0-9]{4}$";
    private const string USERNAME_PATTERN = @"^[a-zA-Z0-9]{4,20}$";
    /*
    ^                         Start anchor
    (?=.*[A-Z])               Ensure string has one uppercase letter.
    (?=.*[!@#$&*])            Ensure string has one special case letter.
    (?=.*[0-9])               Ensure string has one digit.
    (?=.*[a-z].*[a-z].*[a-z]) Ensure string has three lowercase letters.
    .{8,20}                   Ensure string is of length 8-20.
    $                         End anchor.
    */
    private const string PASSWORD_PATTERN = "^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,20}$";
    private const string RANDOM_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static bool IsEmail(string email)
    {
        if (email != null)
        {
            return Regex.IsMatch(email, EMAIL_PATTERN);
        }
        else
        {
            return false;
        }

    }

    public static bool IsUsername(string username)
    {
        if (username != null)
        {
            return Regex.IsMatch(username, USERNAME_PATTERN);
        }
        else
        {
            return false;
        }
    }

    public static bool IsPassword(string password)
    {
        if (password != null)
        {
            return Regex.IsMatch(password, PASSWORD_PATTERN);
        }
        else
        {
            return false;
        }
    }

    public static bool IsUsernameAndDiscriminator(string username)
    {
        if (username != null)
        {
            return Regex.IsMatch(username, USERNAME_AND_DISCRIMINATOR_PATTERN);
        }
        else
        {
            return false;
        }
    }

    public static string GenerateRandom(int length)
    {
        System.Random r = new System.Random();
        return new string(System.Linq.Enumerable.Repeat(RANDOM_CHARS, length).Select(s => s[r.Next(s.Length)]).ToArray());
    }

    public static string Sha256FromString(string toEncrypt)
    {
        var message = Encoding.UTF8.GetBytes(toEncrypt);
        SHA256Managed hashString = new SHA256Managed();

        string hex = "";
        var hashValue = hashString.ComputeHash(message);
        foreach (byte x in hashValue)
        {
            hex += String.Format("{0:x2}", x);
        }

        return hex;
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
    public static ESpellSlot IntToSpellID(int spellID)
    {
        ESpellSlot id = ESpellSlot.NONE;
        switch (spellID)
        {
            case 1:
                id = ESpellSlot.SPELL_1;
                break;
            case 2:
                id = ESpellSlot.SPELL_2;
                break;
            case 3:
                id = ESpellSlot.SPELL_3;
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
            case EControllerID.GAMEPAD_A:
                id = 'A';
                break;
            case EControllerID.GAMEPAD_B:
                id = 'B';
                break;
            case EControllerID.GAMEPAD_C:
                id = 'C';
                break;
            case EControllerID.GAMEPAD_D:
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
                id = EControllerID.GAMEPAD_A;
                break;
            case 'B':
                id = EControllerID.GAMEPAD_B;
                break;
            case 'C':
                id = EControllerID.GAMEPAD_C;
                break;
            case 'D':
                id = EControllerID.GAMEPAD_D;
                break;
        }
        return id;
    }

    /// <summary> Convert an InputButton enum to a SpellID enum </summary>
    public static ESpellSlot GetSpellSlotFrom(EInputButton inputButton)
    {
        ESpellSlot id = ESpellSlot.NONE;
        switch (inputButton)
        {
            case EInputButton.CAST_SPELL_1:
                id = ESpellSlot.SPELL_1;
                break;

            case EInputButton.CAST_SPELL_2:
                id = ESpellSlot.SPELL_2;
                break;

            case EInputButton.CAST_SPELL_3:
                id = ESpellSlot.SPELL_3;
                break;
        }
        return id;
    }

    /// <summary> Convert a TouchJoystickType enum to a SpellID enum </summary>
    public static ESpellSlot GetSpellIDFrom(ETouchJoystickType inputButton)
    {
        ESpellSlot id = ESpellSlot.NONE;
        switch (inputButton)
        {
            case ETouchJoystickType.SPELL_1:
                id = ESpellSlot.SPELL_1;
                break;

            case ETouchJoystickType.SPELL_2:
                id = ESpellSlot.SPELL_2;
                break;

            case ETouchJoystickType.SPELL_3:
                id = ESpellSlot.SPELL_3;
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
    public static EInputButton GetInputButtonFrom(ESpellSlot spellID)
    {
        EInputButton button = EInputButton.NONE;
        switch (spellID)
        {
            case ESpellSlot.SPELL_1:
                button = EInputButton.CAST_SPELL_1;
                break;

            case ESpellSlot.SPELL_2:
                button = EInputButton.CAST_SPELL_2;
                break;

            case ESpellSlot.SPELL_3:
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

    /// <summary> Convert a PlayerID enum to a ClientID enum </summary>
    public static EClientID GetClientIDFrom(EPlayerID playerID)
    {
        EClientID id = EClientID.NONE;
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                id = EClientID.CLIENT_1;
                break;

            case EPlayerID.PLAYER_2:
                id = EClientID.CLIENT_2;
                break;

            case EPlayerID.PLAYER_3:
                id = EClientID.CLIENT_3;
                break;

            case EPlayerID.PLAYER_4:
                id = EClientID.CLIENT_4;
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

    /// <summary> Returns the same team ID as the player ID (e.g. Player 2 -> Team 2)</summary>
    public static EControllerID GetControllerNeteworkID(EPlayerID playerID)
    {
        EControllerID id = EControllerID.NONE;
        switch (playerID)
        {
            case EPlayerID.PLAYER_1:
                id = EControllerID.NETWORK_1;
                break;

            case EPlayerID.PLAYER_2:
                id = EControllerID.NETWORK_2;
                break;

            case EPlayerID.PLAYER_3:
                id = EControllerID.NETWORK_3;
                break;

            case EPlayerID.PLAYER_4:
                id = EControllerID.NETWORK_4;
                break;
        }
        return id;
    }






    #endregion
}
