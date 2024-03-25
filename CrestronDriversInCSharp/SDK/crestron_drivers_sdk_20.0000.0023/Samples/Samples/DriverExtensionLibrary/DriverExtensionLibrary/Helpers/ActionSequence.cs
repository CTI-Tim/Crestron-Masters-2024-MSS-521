// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.

using System;
using System.Collections.Generic;

namespace DriverExtensionLibrary.Helpers
{
    /// <summary>
    /// Wraps multiple action delegates into a single Action in order to extend
    /// callback fields that are Actions instead of multicast delegates.
    /// 
    /// Note: Present implementation allows exceptions to bubble up and doesn't
    /// guarantee that each action gets called if an exception is thrown in the
    /// first one. Users of this class should not rely on this behavior since
    /// it may be changed to handle exceptions more gracefully in the future.
    /// </summary>
    public class ActionSequence
    {
        /// <summary>
        /// Reference to the actions that will be called in order.
        /// </summary>
        /// <remarks>
        /// This is an IEnumerable instead of an array to reinforce that this
        /// collection is not meant to be changed.
        /// </remarks>
        private readonly IEnumerable<Action> Actions;

        /// <summary>
        /// Private constructor to capture the actions. Users of this class
        /// are not expected to have a reference to the class, its only
        /// purpose is to group some Actions into a new Action
        /// </summary>
        /// <param name="actions">The sequence of actions to call in Execute()</param>
        private ActionSequence(IEnumerable<Action> actions)
        {
            Actions = actions;
        }

        /// <summary>
        /// Executes the actions in order. Does not catch exceptions.
        /// </summary>
        private void Execute()
        {
            if (Actions != null)
            {
                foreach (Action action in Actions)
                {
                    if (action != null)
                    {
                        action();
                    }
                }
            }
        }

        /// <summary>
        /// Factory function to chain Actions together
        /// </summary>
        /// <param name="actions">The actions to be called</param>
        /// <returns>A new Action that will call the given actions in the order provided</returns>
        public static Action Chain(params Action[] actions)
        {
            return new ActionSequence(actions).Execute;
        }
    }
}