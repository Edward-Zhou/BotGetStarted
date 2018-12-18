// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using System;

namespace WelcomeUser
{
    public class WelcomeUserStateAccessors
    {
        public WelcomeUserStateAccessors(UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }
        public UserState UserState { get; set; }
        public IStatePropertyAccessor<WelcomeUserState> WelcomeUserState { get; set; }
        public static string WelcomeUserName { get; } = $"{nameof(WelcomeUserStateAccessors)}.WelcomeUserState";
    }
}