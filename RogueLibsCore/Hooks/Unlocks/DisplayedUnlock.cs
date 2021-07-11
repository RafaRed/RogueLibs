﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace RogueLibsCore
{
	/// <summary>
	///   <para>Represents an unlock that can be displayed in the menus.</para>
	/// </summary>
	public abstract class DisplayedUnlock : UnlockWrapper, IComparable<DisplayedUnlock>
	{
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="DisplayedUnlock"/> class with the specified <paramref name="name"/> and <paramref name="type"/>.</para>
		/// </summary>
		/// <param name="name">The name of the unlock.</param>
		/// <param name="type">The type of the unlock.</param>
		/// <param name="unlockedFromStart">Determines whether the unlock is unlocked by default.</param>
		protected DisplayedUnlock(string name, string type, bool unlockedFromStart) : base(name, type, unlockedFromStart) { }
		internal DisplayedUnlock(Unlock unlock) : base(unlock) { }

		/// <summary>
		///   <para>Gets the unlocks menu that the unlock was last associated with.</para>
		/// </summary>
		public UnlocksMenu Menu { get; internal set; }
		/// <summary>
		///   <para>Gets the button data that the unlock was last associated with.</para>
		/// </summary>
		public ButtonData ButtonData { get; internal set; }
		/// <summary>
		///   <para>Gets or sets the menu button's state.</para>
		/// </summary>
		public UnlockButtonState State { get => ButtonData.GetState(); set => ButtonData.SetState(value); }
		/// <summary>
		///   <para>Gets or sets the menu button's text. Supports rich text.</para>
		/// </summary>
		public string Text { get => ButtonData.buttonText; set => ButtonData.buttonText = value; }

		/// <summary>
		///   <para>Gets or sets the unlock's sorting order.</para>
		/// </summary>
		public int SortingOrder { get; set; }
		/// <summary>
		///   <para>Gets or sets the unlock's sorting index.</para>
		/// </summary>
		public int SortingIndex { get; set; }
		/// <summary>
		///   <para>Gets or sets whether the unlock's state (unlocked, purchasable, available or locked) should be ignored during sorting. Default: <see langword="false"/>.</para>
		/// </summary>
		public bool IgnoreStateSorting { get; set; }
		/// <summary>
		///   <para>Compares the current unlock with an <paramref name="other"/> unlock and returns a value that indicates their relative order.</para>
		/// </summary>
		/// <param name="other">The unlock to compare with.</param>
		/// <returns>A value indicating the compared unlocks' relative order.</returns>
		public virtual int CompareTo(DisplayedUnlock other)
		{
			if (other is null) return 1;
			int res = SortingOrder.CompareTo(other.SortingOrder);
			if (res != 0) return res;
			if (!IgnoreStateSorting && !other.IgnoreStateSorting)
			{
				res = GetStateNum().CompareTo(other.GetStateNum());
				if (res != 0) return res;
			}
			res = SortingIndex.CompareTo(other.SortingIndex);
			return res != 0 ? res : Unlock.CompareTo(other.Unlock);
		}
		private int GetStateNum()
		{
			if (IsUnlocked) return 0;
			else if (Unlock.nowAvailable && UnlockCost != 0) return 1;
			else if (Unlock.nowAvailable) return 2;
			else return 3;
		}

		/// <summary>
		///   <para>Updates the unlock's menu button, using the <see cref="UnlockWrapper.IsEnabled"/> property to determine whether the button is selected.</para>
		/// </summary>
		public virtual void UpdateButton() => UpdateButton(IsEnabled, UnlockButtonState.Selected, UnlockButtonState.Normal);
		/// <summary>
		///   <para>Updates the unlock's menu button, using the specified <paramref name="isEnabledOrSelected"/> parameter to determine whether the button is selected.</para>
		/// </summary>
		/// <param name="isEnabledOrSelected">Determines whether the button is selected.</param>
		protected void UpdateButton(bool isEnabledOrSelected) => UpdateButton(isEnabledOrSelected, UnlockButtonState.Selected, UnlockButtonState.Normal);
		/// <summary>
		///   <para>Updates the unlock's menu button, using the specified <paramref name="isEnabledOrSelected"/> parameter to determine the button state between <paramref name="selected"/> and <paramref name="normal"/>.</para>
		/// </summary>
		/// <param name="isEnabledOrSelected">Determines whether <paramref name="selected"/> or <paramref name="normal"/> button state should used.</param>
		/// <param name="selected">Selected button state that is used if <paramref name="isEnabledOrSelected"/> is <see langword="true"/>.</param>
		/// <param name="normal">Normal button state that is used if <paramref name="isEnabledOrSelected"/> is <see langword="false"/>.</param>
		protected void UpdateButton(bool isEnabledOrSelected, UnlockButtonState selected, UnlockButtonState normal)
		{
			Text = GetFancyName();
			State = IsUnlocked ? isEnabledOrSelected ? selected : normal
				: Unlock.nowAvailable && UnlockCost > -1 ? UnlockButtonState.Purchasable
				: UnlockButtonState.Locked;
		}

		/// <summary>
		///   <para>When overriden in a derived class, handles the menu button click.</para>
		/// </summary>
		public abstract void OnPushedButton();

		/// <summary>
		///   <para>Returns the <i>fancy</i> representation of the unlock's name.</para>
		/// </summary>
		/// <returns>The unlock's localized name, with costs, if it's unlocked or can be unlocked; otherwise, "?????", with costs.</returns>
		public virtual string GetFancyName()
		{
			string name = GetName();
			if (Menu.Type == UnlocksMenuType.NewLevelTraits)
			{
				if (Unlock.specialAbilities.Count > 0 || Unlock.leadingTraits.Count > 0)
					name = $"<color=yellow>{name}</color>";
				if (Unlock.isUpgrade)
					name = $"<color=lime>{name}</color>";
				if (Name == "EnduranceTrait" || Name == "Strength" || Name == "Accuracy" || Name == "Speed")
					name = $"<color=cyan>{name}</color>";
				if ((gc.twitchMode || gc.sessionDataBig.twitchOn) && (gc.sessionDataBig.twitchTraits || gc.twitchMode))
				{
					int num = Menu.Unlocks.IndexOf(this);
					int votes = Menu.Agent.isPlayer is 2 ? gc.twitchFunctions.voteCount[num + 5]
						: Menu.Agent.isPlayer is 3 ? gc.twitchFunctions.voteCount[num + 10]
                        : Menu.Agent.isPlayer is 4 ? gc.twitchFunctions.voteCount[num + 15]
						: gc.twitchFunctions.voteCount[num];
					name = $"{name} <color=yellow>#{num + 1 + (Menu.Agent.isPlayer - 1) * 5}</color> <color=cyan>({votes})</color>";
				}
			}
			else if (Menu.Type == UnlocksMenuType.TwitchRewards)
			{
				if (gc.twitchMode || gc.sessionDataBig.twitchOn && gc.sessionDataBig.twitchRewards)
				{
					int num = Menu.Unlocks.IndexOf(this);
					name = $"{name} <color=yellow>#{num + 1}</color> <color=cyan>({gc.twitchFunctions.voteCount[num]})</color>";
				}
			}
			else if (Menu.Type == UnlocksMenuType.TwitchDisasters)
			{
				if (gc.twitchMode || gc.sessionDataBig.twitchOn && gc.sessionDataBig.twitchRewards)
				{
					int num = Menu.Unlocks.IndexOf(this);
					name = $"{name} <color=yellow>#{num + 1}</color> <color=cyan>({gc.twitchFunctions.voteCount[num]})</color>";
				}
			}
			else if (Menu.Type == UnlocksMenuType.CharacterCreation)
			{
				if (CharacterCreationCost != 0)
					name += $" | <color={(CharacterCreationCost < 0 ? "lime" : "orange")}>{CharacterCreationCost}</color>";
			}
			else if (Unlock.nowAvailable && UnlockCost > -1)
			{
				name += $" - ${UnlockCost}";
			}
			return name;
		}
		/// <summary>
		///   <para>Returns the <i>fancy</i> representation of the unlock's description.</para>
		/// </summary>
		/// <returns>The unlock's localized description, with cancellations and recommendations, if it's unlocked; otherwise, the unlock's localized description, with cancellations, recommendations and prerequisites, if it can be unlocked; otherwise, "?????", with prerequisites.</returns>
		public virtual string GetFancyDescription()
		{
			if (IsUnlocked || Unlock.nowAvailable)
			{
				string text = GetDescription();
				AddCancellationsTo(ref text);
				AddRecommendationsTo(ref text);
				if (!IsUnlocked || RogueFramework.IsDebugEnabled(DebugFlags.Unlocks | DebugFlags.UnlockMenus))
					AddPrerequisitesTo(ref text);
				return text;
			}
			else
			{
				string text = "?????";
				AddPrerequisitesTo(ref text);
				return text;
			}
		}

		/// <summary>
		///   <para>Adds cancellations to the end of the specified <paramref name="description"/>.</para>
		/// </summary>
		/// <param name="description">The description to append to.</param>
		protected void AddCancellationsTo(ref string description)
		{
			if (description is null) description = string.Empty;
			else description += "\n\n";

			if (Unlock.cancellations.Count > 0)
			{
				description += $"<color=orange>{gc.nameDB.GetName("Cancels", "Interface")}:</color>\n" +
					string.Join(", ", Unlock.cancellations.ConvertAll(unlockName =>
					{
						UnlockWrapper unlock = (UnlockWrapper)gc.sessionDataBig.unlocks.Find(u => u.unlockName == unlockName)?.__RogueLibsCustom;
						return unlock?.GetName();
					}));
			}
		}
		/// <summary>
		///   <para>Adds recommendations to the end of the specified <paramref name="description"/>.</para>
		/// </summary>
		/// <param name="description">The description to append to.</param>
		protected void AddRecommendationsTo(ref string description)
		{
			if (description is null) description = string.Empty;
			else description += "\n\n";

			if (Unlock.recommendations.Count > 0)
			{
				description += $"<color=cyan>{gc.nameDB.GetName("Recommends", "Interface")}:</color>\n" +
					string.Join(", ", Unlock.recommendations.ConvertAll(unlockName =>
					{
						UnlockWrapper unlock = (UnlockWrapper)gc.sessionDataBig.unlocks.Find(u => u.unlockName == unlockName)?.__RogueLibsCustom;
						return unlock?.GetName();
					}));
			}
		}
		/// <summary>
		///   <para>Adds prerequisites to the end of the specified <paramref name="description"/>.</para>
		/// </summary>
		/// <param name="description">The description to append to.</param>
		protected void AddPrerequisitesTo(ref string description)
		{
			if (description is null) description = string.Empty;
			else description += "\n\n";

			List<string> prereqs = new List<string>();
			if (Unlock.prerequisites.Count > 0)
			{
				prereqs.Add(string.Join(", ", Unlock.prerequisites.ConvertAll(unlockName =>
				{
					UnlockWrapper unlock = (UnlockWrapper)gc.sessionDataBig.unlocks.Find(u => u.unlockName == unlockName)?.__RogueLibsCustom;
					if (unlock != null)
					{
						string name = unlock.GetName();
						if (unlock.IsUnlocked) name = $"<color=#EEEEEE55>{name}</color>";
						return name;
					}
					return unlockName;
				})));
			}
			if (Unlock.cost is -2)
			{
				prereqs.Add(gc.unlocks.GetSpecialUnlockInfo(Name, Unlock));
			}
			if (Unlock.cost > 0)
			{
				string costColor = gc.sessionDataBig.nuggets >= Unlock.cost ? "cyan" : "red";
				prereqs.Add($"\n{gc.nameDB.GetName("UnlockFor", "Unlock")} <color={costColor}>${Unlock.cost}</color>");
			}
			if (prereqs.Count > 0)
				description += $"<color=cyan>{gc.nameDB.GetName("Prerequisites", "Unlock")}:</color>\n" + string.Join("\n", prereqs);
		}

		/// <summary>
		///   <para>Plays an audio clip with the specified <paramref name="clipName"/> in the menu.</para>
		/// </summary>
		/// <param name="clipName">The name of the audio clip to play.</param>
		protected void PlaySound(string clipName) => Menu.PlaySound(clipName);
		/// <summary>
		///   <para>Sends the specified <paramref name="msg1"/> in the chat as an announcement, with <paramref name="msg2"/> and <paramref name="msg3"/> specifying additional information.</para>
		/// </summary>
		/// <param name="msg1">The announcement message identifier.</param>
		/// <param name="msg2">The first of the additional identifiers.</param>
		/// <param name="msg3">The second of the additional identifiers.</param>
		protected void SendAnnouncementInChat(string msg1, string msg2 = null, string msg3 = null)
		{
			if (gc.serverPlayer && gc.multiplayerMode)
				Menu.Agent.objectMult.SendChatAnnouncement(msg1, msg2 ?? string.Empty, msg3 ?? string.Empty);
		}

		/// <summary>
		///   <para>Updates the current menu.</para>
		/// </summary>
		public void UpdateMenu() => Menu.UpdateMenu();
		/// <summary>
		///   <para>Updates all unlocks in the current menu.</para>
		/// </summary>
		public void UpdateAllUnlocks()
		{
			foreach (DisplayedUnlock unlock in Menu.Unlocks)
			{
				unlock.UpdateUnlock();
				unlock.UpdateButton();
			}
		}
		/// <summary>
		///   <para>Returns an enumerable collection of unlocks conflicting with the current unlock.</para>
		/// </summary>
		/// <returns>An enumerable collection of <see cref="DisplayedUnlock"/>s representing conflicting unlocks.</returns>
		public IEnumerable<DisplayedUnlock> EnumerateCancellations()
		{
			foreach (DisplayedUnlock unlock in Menu.Unlocks)
				if (unlock.Unlock.cancellations.Contains(Name) || Unlock.cancellations.Contains(unlock.Name))
					yield return unlock;
		}
	}
}