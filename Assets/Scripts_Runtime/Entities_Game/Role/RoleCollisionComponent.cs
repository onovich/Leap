using System;
using UnityEngine;

namespace Leap {

    public class RoleCollisionComponent : MonoBehaviour {

        public event Action<Collider2D> OnTriggerEnterHandle;
        public event Action<Collider2D> OnTriggerStayHandle;
        public event Action<Collider2D> OnTriggerExitHandle;

        public event Action<Collision2D> OnCollisionEnterHandle;
        public event Action<Collision2D> OnCollisionStayHandle;
        public event Action<Collision2D> OnCollisionExitHandle;

        void OnTriggerEnter2D(Collider2D other) {
            OnTriggerEnterHandle?.Invoke(other);
        }

        void OnTriggerStay2D(Collider2D other) {
            OnTriggerStayHandle?.Invoke(other);
        }

        void OnTriggerExit2D(Collider2D other) {
            OnTriggerExitHandle?.Invoke(other);
        }

        void OnCollisionEnter2D(Collision2D other) {
            OnCollisionEnterHandle?.Invoke(other);
        }

        void OnCollisionStay2D(Collision2D other) {
            OnCollisionStayHandle?.Invoke(other);
        }

        void OnCollisionExit2D(Collision2D other) {
            OnCollisionExitHandle?.Invoke(other);
        }

        public void TearDown() {
            OnTriggerEnterHandle = null;
            OnTriggerStayHandle = null;
            OnTriggerExitHandle = null;
            OnCollisionEnterHandle = null;
            OnCollisionStayHandle = null;
            OnCollisionExitHandle = null;
            Destroy(this.gameObject);
        }

    }

}