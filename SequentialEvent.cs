using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zinnia.Action;

namespace JorgeJGnz
{
    public enum SeqMode
    {
        CheckAfterDelay,
        WaitUntilTimeout
    }

    public class SequentialEvent : BooleanAction
    {
        [Header("Phase 0 - Detection")]
        public BooleanAction primaryCondition;
        public List<BooleanAction> AndTheseDisabled;

        [Header("Phase 1 - Intention")]
        public float intentionCheckingDelay;
        public UnityEngine.Events.UnityEvent onIntentionValidated;
        public UnityEngine.Events.UnityEvent onIntentionFailed;
        public bool sequenceInterruptionIsFailure = false;

        [Header("Phase 2 - Validation")]
        public BooleanAction secondaryCondition;
        public bool expectedValue = true;
        public SeqMode mode;

        [Header("If checking after delay")]
        public float validationCheckingDelay;

        [Header("If waiting until timeout")]
        public float validationTimeout = 1.0f;
        public float validationCheckingFreq = 0.05f;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent onSuccess;
        public UnityEngine.Events.UnityEvent onFailure;

        [Header("Debugging")]
        public bool started = false;

        void Update()
        {

            bool OthersAreDisabled = true;
            for (int i = 0; i < AndTheseDisabled.Count; i++)
            {
                if (AndTheseDisabled[i].Value)
                {
                    OthersAreDisabled = false;
                    break;
                }
            }

            if (primaryCondition.Value && OthersAreDisabled && !started)
            {
                // Is there intention to perform this sequential gesture?
                StartCoroutine(IntentionChecking()); 
                started = true;
            }

            Receive(started);
            
        }

        IEnumerator IntentionChecking()
        {
            yield return new WaitForSeconds(intentionCheckingDelay);
			
			// If, after some time, initial gesture is still being held then intention is validated
            if (primaryCondition.Value)
            {
                onIntentionValidated.Invoke();

                // Secondary action validation
                switch (mode)
                {
                    case SeqMode.CheckAfterDelay:
                        StartCoroutine(SecondaryActionValidationAfterDelay(validationCheckingDelay));
                        break;
                    case SeqMode.WaitUntilTimeout:
                        StartCoroutine(SecondaryActionValidationUntilTimeout(validationTimeout, validationCheckingFreq));
                        break;
                }
            }
            else
            {
                // There was no intention
                onIntentionFailed.Invoke();

                started = false;
            }
        }

        IEnumerator SecondaryActionValidationAfterDelay(float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);

            if (secondaryCondition.Value == expectedValue) onSuccess.Invoke();
            else onFailure.Invoke();

            started = false;
        }

        IEnumerator SecondaryActionValidationUntilTimeout(float timeout, float checkEvery)
        {
            while (timeout > 0.0f && !secondaryCondition.Value)
            {
                yield return new WaitForSeconds(checkEvery);
                timeout -= checkEvery;
            }

            if (secondaryCondition.Value == expectedValue) onSuccess.Invoke();
            else onFailure.Invoke();

            started = false;
        }

        public void Interrupt()
        {
            StopAllCoroutines();

            if (sequenceInterruptionIsFailure) onFailure.Invoke();

            started = false;
        }
    }
}
