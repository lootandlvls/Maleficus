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
using System.Linq.Expressions;

namespace Maleficus
{
    /// <summary>
    /// Contains utility functions.
    /// </summary>
    public static class Utils
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
        public const string EMAIL_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
        public const string USERNAME_PATTERN = @"^[a-zA-Z0-9!#$%&*?^+_~.,=-]{1,18}$";
        public const string USERNAME_PLAYER_PATTERN = "^(player_[0-9]{1,14})$";
        //private const string PASSWORD_PATTERN = "^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,20}$";
        public const string PASSWORD_PATTERN = @"^[a-zA-Z0-9!#$%&*?^+_~.,=-]{5,20}$";
        public const string RANDOM_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

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
                if(!Regex.IsMatch(username, USERNAME_PLAYER_PATTERN))
                {
                    return Regex.IsMatch(username, USERNAME_PATTERN);
                }
            }
            return false;
        }

        public static bool IsPassword(string password)
        {
            if (password != null)
            {
                return Regex.IsMatch(password, PASSWORD_PATTERN);
            }
            return false;
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

        /// <summary>
        /// Converts the system time from System.DateTime into an integer
        /// </summary>
        /// <returns> 3 first digits : milliseconds, 4th and 5th : seconds, 6th and 7th : minutes, 8th and 9th : hours </returns>
        public static int GetSystemTime()
        {
            return DateTime.Now.Millisecond + DateTime.Now.Second * 1000 + DateTime.Now.Minute * 100000 + DateTime.Now.Hour * 10000000;
        }

        #endregion

        #region Maleficus Types Conversions
        /// <summary> Convert a PlayerID enum to an int </summary>
        public static int GetIntFrom(EPlayerID playerID)
        {
            int result = 0;
            switch (playerID)
            {
                case EPlayerID.PLAYER_1:
                    result = 1;
                    break;
                case EPlayerID.PLAYER_2:
                    result = 2;
                    break;
                case EPlayerID.PLAYER_3:
                    result = 3;
                    break;
                case EPlayerID.PLAYER_4:
                    result = 4;
                    break;
            }
            return result;
        }

        /// <summary> Convert an int to a PlayerID enum </summary>
        public static EPlayerID GetPlayerIDFrom(int playerID)
        {
            EPlayerID result = EPlayerID.NONE;
            switch (playerID)
            {
                case 1:
                    result = EPlayerID.PLAYER_1;
                    break;
                case 2:
                    result = EPlayerID.PLAYER_2;
                    break;
                case 3:
                    result = EPlayerID.PLAYER_3;
                    break;
                case 4:
                    result = EPlayerID.PLAYER_4;
                    break;
            }
            return result;
        }
        /// <summary> Convert an int to a ClientID enum </summary>
        public static EClientID GetClienIDFrom(int clientID)
        {
            EClientID result = EClientID.NONE;
            switch (clientID)
            {
                case 0:
                    result = EClientID.SERVER;
                    break;
                case 1:
                    result = EClientID.CLIENT_1;
                    break;
                case 2:
                    result = EClientID.CLIENT_2;
                    break;
                case 3:
                    result = EClientID.CLIENT_3;
                    break;
                case 4:
                    result = EClientID.CLIENT_4;
                    break;
            }
            return result;
        }

        /// <summary> Convert an int to a SpellID enum (1 being the spell slot 1) </summary>
        public static ESpellSlot GetSpellSlotFrom(int spellID)
        {
            ESpellSlot result = ESpellSlot.NONE;
            switch (spellID)
            {
                case 1:
                    result = ESpellSlot.SPELL_1;
                    break;
                case 2:
                    result = ESpellSlot.SPELL_2;
                    break;
                case 3:
                    result = ESpellSlot.SPELL_3;
                    break;
            }
            return result;
        }

        /// <summary> Convert a ControllerID enum to an Char </summary>
        public static char GetCharFrom(EControllerID ControllerID)
        {
            char result = 'X';
            switch (ControllerID)
            {
                case EControllerID.GAMEPAD_A:
                    result = 'A';
                    break;
                case EControllerID.GAMEPAD_B:
                    result = 'B';
                    break;
                case EControllerID.GAMEPAD_C:
                    result = 'C';
                    break;
                case EControllerID.GAMEPAD_D:
                    result = 'D';
                    break;
            }
            return result;
        }

        /// <summary> Convert a Char to a ControllerID enum </summary>
        public static EControllerID GetControllerIDFrom(char ControllerID)
        {
            EControllerID result = EControllerID.NONE;
            switch (ControllerID)
            {
                case 'A':
                    result = EControllerID.GAMEPAD_A;
                    break;
                case 'B':
                    result = EControllerID.GAMEPAD_B;
                    break;
                case 'C':
                    result = EControllerID.GAMEPAD_C;
                    break;
                case 'D':
                    result = EControllerID.GAMEPAD_D;
                    break;
            }
            return result;
        }

        /// <summary> Convert a joystickType enum to a char</summary>
        public static char GetCharFrom(EJoystickType joystickType)
        {
            char resut = 'X';
            switch (joystickType)
            {
                case EJoystickType.MOVEMENT:
                    resut = 'L';
                    break;

                case EJoystickType.ROTATION:
                    resut = 'R';
                    break;
            }
            return resut;
        }

        /// <summary> Convert an InputButton enum to a SpellID enum </summary>
        public static ESpellSlot GetSpellSlotFrom(EInputButton inputButton)
        {
            ESpellSlot result = ESpellSlot.NONE;
            switch (inputButton)
            {
                case EInputButton.CAST_SPELL_1:
                    result = ESpellSlot.SPELL_1;
                    break;

                case EInputButton.CAST_SPELL_2:
                    result = ESpellSlot.SPELL_2;
                    break;

                case EInputButton.CAST_SPELL_3:
                    result = ESpellSlot.SPELL_3;
                    break;
            }
            return result;
        }

        /// <summary> Convert a SpellSlot enum to an int </summary>
        public static int GetIntFrom(ESpellSlot spellSlot)
        {
            int result = 0;
            switch (spellSlot)
            {
                case ESpellSlot.SPELL_1:
                    result = 1;
                    break;

                case ESpellSlot.SPELL_2:
                    result = 2;
                    break;

                case ESpellSlot.SPELL_3:
                    result = 3;
                    break;
            }
            return result;
        }

        /// <summary> Convert a TouchJoystickType enum to a SpellID enum </summary>
        public static ESpellSlot GetSpellSlotFrom(ETouchJoystickType inputButton)
        {
            ESpellSlot result = ESpellSlot.NONE;
            switch (inputButton)
            {
                case ETouchJoystickType.SPELL_1:
                    result = ESpellSlot.SPELL_1;
                    break;

                case ETouchJoystickType.SPELL_2:
                    result = ESpellSlot.SPELL_2;
                    break;

                case ETouchJoystickType.SPELL_3:
                    result = ESpellSlot.SPELL_3;
                    break;
            }
            return result;
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
            EInputButton result = EInputButton.NONE;
            switch (spellID)
            {
                case ESpellSlot.SPELL_1:
                    result = EInputButton.CAST_SPELL_1;
                    break;

                case ESpellSlot.SPELL_2:
                    result = EInputButton.CAST_SPELL_2;
                    break;

                case ESpellSlot.SPELL_3:
                    result = EInputButton.CAST_SPELL_3;
                    break;
            }
            return result;
        }

        /// <summary> Convert a ClientID enum to a PlayerID enum </summary>
        public static EPlayerID GetPlayerIDFrom(EClientID clientID)
        {
            EPlayerID result = EPlayerID.NONE;
            switch (clientID)
            {
                case EClientID.CLIENT_1:
                    result = EPlayerID.PLAYER_1;
                    break;

                case EClientID.CLIENT_2:
                    result = EPlayerID.PLAYER_2;
                    break;

                case EClientID.CLIENT_3:
                    result = EPlayerID.PLAYER_3;
                    break;

                case EClientID.CLIENT_4:
                    result = EPlayerID.PLAYER_4;
                    break;
            }
            return result;
        }

        /// <summary> Convert a PlayerID enum to a ClientID enum </summary>
        public static EClientID GetClientIDFrom(EPlayerID playerID)
        {
            EClientID result = EClientID.NONE;
            switch (playerID)
            {
                case EPlayerID.PLAYER_1:
                    result = EClientID.CLIENT_1;
                    break;

                case EPlayerID.PLAYER_2:
                    result = EClientID.CLIENT_2;
                    break;

                case EPlayerID.PLAYER_3:
                    result = EClientID.CLIENT_3;
                    break;

                case EPlayerID.PLAYER_4:
                    result = EClientID.CLIENT_4;
                    break;
            }
            return result;
        }

        /// <summary> Returns the same team ID as the player ID (e.g. Player 2 -> Team 2)</summary>
        public static ETeamID GetIdenticPlayerTeam(EPlayerID playerID)
        {
            ETeamID result = ETeamID.NONE;
            switch (playerID)
            {
                case EPlayerID.PLAYER_1:
                    result = ETeamID.TEAM_1;
                    break;

                case EPlayerID.PLAYER_2:
                    result = ETeamID.TEAM_2;
                    break;

                case EPlayerID.PLAYER_3:
                    result = ETeamID.TEAM_3;
                    break;

                case EPlayerID.PLAYER_4:
                    result = ETeamID.TEAM_4;
                    break;
            }
            return result;
        }

        /// <summary> Converts a playerID to a network controller </summary>
        public static EControllerID GetControllerNeteworkID(EPlayerID playerID)
        {
            EControllerID result = EControllerID.NONE;
            switch (playerID)
            {
                case EPlayerID.PLAYER_1:
                    result = EControllerID.NETWORK_1;
                    break;

                case EPlayerID.PLAYER_2:
                    result = EControllerID.NETWORK_2;
                    break;

                case EPlayerID.PLAYER_3:
                    result = EControllerID.NETWORK_3;
                    break;

                case EPlayerID.PLAYER_4:
                    result = EControllerID.NETWORK_4;
                    break;
            }
            return result;
        }

        /// <summary> Converts a controllerID (AI or Network) to a PlayerID </summary>
        public static EPlayerID GetPlayerIDFrom(EControllerID controllerID)
        {
            EPlayerID result = EPlayerID.NONE;
            switch (controllerID)
            {
                /* AI */
                case EControllerID.AI_1:
                    result = EPlayerID.PLAYER_1;
                    break;

                case EControllerID.AI_2:
                    result = EPlayerID.PLAYER_2;
                    break;

                case EControllerID.AI_3:
                    result = EPlayerID.PLAYER_3;
                    break;

                case EControllerID.AI_4:
                    result = EPlayerID.PLAYER_4;
                    break;

                /* Network */
                case EControllerID.NETWORK_1:
                    result = EPlayerID.PLAYER_1;
                    break;

                case EControllerID.NETWORK_2:
                    result = EPlayerID.PLAYER_2;
                    break;

                case EControllerID.NETWORK_3:
                    result = EPlayerID.PLAYER_3;
                    break;

                case EControllerID.NETWORK_4:
                    result = EPlayerID.PLAYER_4;
                    break;
            }
            return result;
        }




        /// <summary> Converts an array with 3 elements to a Vector3 </summary>
        public static Vector3 GetVectorFrom(float[] vector)
        {
            if (vector.Length != 3)
            {
                Debug.LogError("parameter vector should contain exactly 3 elements");
                return new Vector3();
            }

            return new Vector3(vector[0], vector[1], vector[2]);
        }



        #endregion
    }
}