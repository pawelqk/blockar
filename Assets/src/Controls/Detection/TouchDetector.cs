using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Utils.StateMachine;

namespace Controls.Detection
{
    public class TouchDetector : IStateContext
    {
        private const string VIRTUAL_OBJECT_TAG = "InstantiatedVirtualObject";
        private const int UI_LAYER = 5;
        private const float SCREEN_TOP_LIMIT = 250.0f;
        private const float SCREEN_BOTTOM_LIMIT = 300.0f;

        private readonly PlaneTouchHandler planeTouchHandler;
        private readonly VirtualObjectTouchHandler virtualObjectTouchHandler;
        private readonly ARRaycastManager arRaycastmanager;
        private readonly Logger logger;
        private TouchStateBase touchState;
        private Touch? lastTouch;
        private bool lastTouchResolved;

        public TouchDetector(PlaneTouchHandler planeTouchHandler, VirtualObjectTouchHandler virtualObjectTouchHandler,
            ARRaycastManager arRaycastmanager, Logger logger)
        {
            this.planeTouchHandler = planeTouchHandler;
            this.virtualObjectTouchHandler = virtualObjectTouchHandler;
            this.arRaycastmanager = arRaycastmanager;
            this.logger = logger;
            this.lastTouch = null;
            this.lastTouchResolved = false;
            GenericState<TouchDetector>.InitState<SNoTouch>(this);
        }

        public Touch? LastTouch { get => lastTouch; }

        public bool CheckForTouch()
        {
            if (Input.touchCount == 0)
            {
                lastTouch = null;
                return false;
            }

            lastTouch = Input.GetTouch(0);
            logger.Log("CheckForTouch()", String.Format("lastTouch: {0}", ((Touch)lastTouch).phase));

            return true;
        }

        public bool ResolveLastTouch()
        {
            if (lastTouch is null)
                return false;

            lastTouchResolved = false;
            touchState.HandleTouch((Touch)lastTouch);

            logger.LogWarning("ResolveLastTouch", String.Format("lastTouchResolved: {0}", lastTouchResolved));
            return lastTouchResolved;
        }

        protected void OnTap()
        {
            if(IsUITouch())
            {
                lastTouchResolved = true;
                return;
            }

            if (TryToResolveVirtualObjectTap())
            {
                lastTouchResolved = true;
                return;
            }

            if (TryToResolvePlaneTap())
            {
                lastTouchResolved = true;
                return;
            }

            lastTouchResolved = false;
        }

        private bool IsUITouch()
        {
            return IsUIElementTouch() || IsScreenTopTouch() || IsScreenBottomTouch();
        }

        private bool IsScreenTopTouch()
        {
            // Top swipe is required to show Android's top menu, such gesture shouldn't be classfied as "game interaction"
            var touch = (Touch)lastTouch;
            var isScreenTopTouch = (Screen.height - touch.position.y) < SCREEN_TOP_LIMIT;
            logger.Log("IsScreenTopTouch()", String.Format("IsScreenTopTouch: {0}", isScreenTopTouch));
            return isScreenTopTouch;
        }

        private bool IsScreenBottomTouch()
        {
            // Mobiles with virtual buttons requires finger swipe at screen's bottom area
            // such swipes shouldn't be classfied as "game interaction"
            var touch = (Touch)lastTouch;
            var isScreenBottomTouch = touch.position.y < SCREEN_BOTTOM_LIMIT;
            logger.Log("IsScreenBottomTouch()", String.Format("isScreenBottomTouch: {0}", isScreenBottomTouch));
            return isScreenBottomTouch;
        }

        private bool IsUIElementTouch()
        {
            var touch = (Touch)lastTouch;
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = touch.position;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            foreach (RaycastResult raycastResult in raycastResults)
            {
                if (raycastResult.gameObject.layer == UI_LAYER)
                {
                    logger.Log("IsUIElementTouch()", "UI element touch detected");
                    return true;
                }
            }

            logger.Log("IsUIElementTouch()", "NOT UI element touch");
            return false;
        }

        private bool TryToResolvePlaneTap()
        {
            logger.Log("TryToResolvePlaneTap", "");
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            var touch = (Touch)lastTouch;
            if (!arRaycastmanager.Raycast(touch.position, hits, TrackableType.Planes))
                return false;

            planeTouchHandler.OnPlaneTouch(hits[0]);
            logger.Log("TryToResolvePlaneTap", "tap resolved");
            return true;
        }

        private bool GetVirtualObjectHit(out RaycastHit hit)
        {
            var touch = (Touch)lastTouch;
            Vector3 pos = touch.position;
            Ray ray = Camera.main.ScreenPointToRay(pos);
            return Physics.Raycast(ray, out hit) && (hit.collider.tag.Equals(VIRTUAL_OBJECT_TAG));
        }

        private bool TryToResolveVirtualObjectTap()
        {
            logger.Log("TryToResolveVirtualObjectTap", "");
            if (!GetVirtualObjectHit(out RaycastHit hit))
                return false;

            virtualObjectTouchHandler.OnVirtualObjectTap(hit);
            logger.Log("TryToResolveVirtualObjectTap", "tap resolved");
            return true;
        }

        protected void OnHold()
        {
            if (IsUITouch())
            {
                lastTouchResolved = true;
                return;
            }

            if (TryToResolveVirtualObjectHold())
            {
                lastTouchResolved = true;
                return;
            }

            if (TryToResolvePlaneHold())
            {
                lastTouchResolved = true;
                return;
            }

            lastTouchResolved = false;
        }

        private bool TryToResolveVirtualObjectHold()
        {
            logger.Log("TryToResolveVirtualObjectHold", "");
            if (!GetVirtualObjectHit(out RaycastHit hit))
                return false;

            virtualObjectTouchHandler.OnVirtualObjectHold(hit);
            logger.Log("TryToResolveVirtualObjectHold", "hold resolved");
            return true;
        }

        private bool TryToResolvePlaneHold()
        {
            logger.Log("TryToResolvePlaneHold", "hold resolved");
            planeTouchHandler.OnPlaneHold();
            return true;
        }

        public void SetState<ConcreteContext>(GenericState<ConcreteContext> state) where ConcreteContext : IStateContext
        {
            this.touchState = (TouchStateBase) (object) state;
        }


        // ===========================================================================
        // State machine ... C# limitations forces to put that code in the same file
        // ===========================================================================
        protected abstract class TouchStateBase : GenericState<TouchDetector>
        {
            protected override void OnEnter()
            {
            }

            protected override void OnExit()
            {
            }

            public virtual void HandleTouch(Touch touch)
            {
            }
        }

        protected class SNoTouch : TouchStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("SNoTouch::OnEnter", "");
            }

            public override void HandleTouch(Touch touch)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Change<STapStarted>();
                        break;
                    case TouchPhase.Stationary:
                        Change<STapHolding>();
                        break;
                    case TouchPhase.Moved:
                        Change<STapMoved>();
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                    default:
                        context.logger.LogWarning(
                            "SNoTouch::HandleTouch",
                            String.Format("TouchPhase: {0} was unexpected in this state. Ignoring it.", touch.phase)
                        );
                        break;
                }
            }
        }

        protected class STapStarted : TouchStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("STapStarted::OnEnter", "");
            }

            public override void HandleTouch(Touch touch)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Stationary:
                        Change<STapHolding>();
                        break;
                    case TouchPhase.Moved:
                        Change<STapMoved>();
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        Change<STapReleased>();
                        break;
                    default:
                        break;
                }
            }
        }

        protected class STapMoved : TouchStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("STapMoved::OnEnter", "");
            }

            public override void HandleTouch(Touch touch)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Change<STapStarted>();
                        break;
                    case TouchPhase.Stationary:
                        Change<STapHolding>();
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        Change<STapReleased>();
                        break;
                    default:
                        break;
                }
            }
        }

        protected class STapReleased : TouchStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("STapReleased::OnEnter", "");
                Change<SNoTouch>(); // immediate Change to SNoTouch
            }

            protected override void OnExit()
            {
                context.logger.Log("STapReleased::OnExit", "Calling context.OnTap()");
                context.OnTap();
            }
        }

        protected class STapHolding : TouchStateBase
        {
            private readonly Stopwatch stopwatch = new Stopwatch();
            private const long HOLD_THRESHOLD_MS = 500;
            protected override void OnEnter()
            {
                context.logger.Log("STapHolding::OnEnter", "");
                stopwatch.Start();
            }

            public override void HandleTouch(Touch touch)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Change<STapStarted>();
                        break;
                    case TouchPhase.Moved:
                        Change<STapMoved>();
                        break;
                    case TouchPhase.Stationary:
                        HandleTouchStationary();
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        HandleTouchEnded();
                        break;
                    default:
                        break;
                }
            }

            private bool IsHoldLongEnough(long elapsedMs)
            {
                return elapsedMs >= HOLD_THRESHOLD_MS;
            }

            private long PauseStopwatch()
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                context.logger.Log("PauseStopwatch", String.Format("Elapsed holding time in ms: {0}", elapsedMs));
                return elapsedMs;
            }

            private void ResumeStopwatch()
            {
                context.logger.Log("ResumeStopwatch", "");
                stopwatch.Start();
            }

            private void HandleTouchEnded()
            {
                var elapsedMs = PauseStopwatch();
                if (IsHoldLongEnough(elapsedMs))
                    Change<SHoldReleased>();
                else
                    Change<STapReleased>();
            }

            private void HandleTouchStationary()
            {
                var elapsedMs = PauseStopwatch();
                if (IsHoldLongEnough(elapsedMs))
                    Change<SHoldAcquired>();
                else
                    ResumeStopwatch();
            }
        }

        protected class SHoldReleased : TouchStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("SHoldReleased::OnEnter", "");
                Change<SNoTouch>(); // immediate Change to SNoTouch
            }

            protected override void OnExit()
            {
                context.logger.Log("SHoldReleased::OnExit", "Calling context.OnHold()");
                context.OnHold();
            }
        }

        protected class SHoldAcquired : TouchStateBase
        {
            protected override void OnEnter()
            {
                context.logger.Log("SHoldAcquired::OnEnter", "Calling context.OnHold()");
                context.OnHold();
            }

            protected override void OnExit()
            {
                context.logger.Log("SHoldAcquired::OnExit", "");
            }

            public override void HandleTouch(Touch touch)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Change<STapStarted>();
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        Change<SNoTouch>();
                        break;
                    default:
                        context.logger.Log(
                            "HandleTouch",
                            String.Format("touchPhase: {0}, statying in SHoldAcquired until Ended / Canceled received", touch.phase)
                        );
                        break;
                }
            }
        }
    }
}
