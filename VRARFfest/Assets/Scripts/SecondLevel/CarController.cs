    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;
    using UnityEngine.XR.Interaction.Toolkit.Interactables;
    using UnityEngine.XR.Interaction.Toolkit.Interactors;

    public class CarController : MonoBehaviour
    {
        public float moveSpeed;
        public AudioSource expl;
        public float boostMultiplier = 2f;

        public float slowdownMultiplier = 0.2f;

        public float minSwipeSpeed = 1f;

        public float boostDuration = 1f;
        public float speedDecayRate = 2f;

        public float scale;
        public float min_scale_coef;
        public float max_scale_coef;
        private float currentSpeed;
        private float boostTimer = 0f;
        private bool isBoosted = false;
        private bool isSlowed = false;
        private XRGrabInteractable grabInteractable;
        private IXRSelectInteractor currentInteractor;
        private Vector3 lastControllerPosition;
        private bool hasLastPosition = false;

        void Start()
        {
            currentSpeed = moveSpeed;

            grabInteractable = GetComponent<XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
            }

            grabInteractable.selectEntered.AddListener(OnGrabStarted);
            grabInteractable.selectExited.AddListener(OnGrabEnded);
            scale = Random.Range(min_scale_coef, max_scale_coef);
            transform.localScale = new Vector3(transform.localScale.x * scale, scale * transform.localScale.x * scale, scale * transform.localScale.x * scale);
            moveSpeed = Random.Range(0.8f, 1.5f);
        }

        void Update()
        {

            if (currentInteractor != null && currentInteractor.isSelectActive)
            {
                Transform interactorTransform = currentInteractor.transform;
                if (interactorTransform != null)
                {
                    Vector3 controllerPosition = interactorTransform.position;
                
                    if (hasLastPosition)
                    {

                        Vector3 controllerMovement = controllerPosition - lastControllerPosition;

                        Vector3 localMovement = transform.InverseTransformDirection(controllerMovement);

                        float swipeSpeed = localMovement.z / Time.deltaTime;

                        if (Mathf.Abs(swipeSpeed) > minSwipeSpeed)
                        {
                            if (swipeSpeed > 0f)
                            {

                                isBoosted = true;
                                isSlowed = false;
                                boostTimer = boostDuration;
                                gameObject.GetComponent<AudioSource>().Play();
                            }
                            else
                            {
                                isSlowed = true;
                                isBoosted = false;
                                boostTimer = boostDuration;
                            }
                        }
                    }
                    else
                    {
                        hasLastPosition = true;
                    }
                
                    lastControllerPosition = controllerPosition;
                }
            }
            else
            {
                hasLastPosition = false;
            }

            if (boostTimer > 0f)
            {
                boostTimer -= Time.deltaTime;
            
                if (boostTimer <= 0f)
                {
                    isBoosted = false;
                    isSlowed = false;
                    boostTimer = 0f;
                }
            }

            float targetSpeed = moveSpeed + 0.001f;
            if (isBoosted)
            {
                targetSpeed = moveSpeed * boostMultiplier;
            }
            else if (isSlowed)
            {
                targetSpeed = moveSpeed * slowdownMultiplier;
            }
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedDecayRate);
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
        }

        void FixedUpdate()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        private void OnGrabStarted(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject;
            hasLastPosition = false;
        
            if (currentInteractor != null && currentInteractor.transform != null)
            {
                lastControllerPosition = currentInteractor.transform.position;
            }
        }
        private void OnGrabEnded(SelectExitEventArgs args)
        {
            currentInteractor = null;
            hasLastPosition = false;
        }

        void OnDestroy()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
                grabInteractable.selectExited.RemoveListener(OnGrabEnded);
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Car"))
            {
                Debug.Log("Collision with car");
                expl.Play();
            PerekrestokLevelController.IsGameEnded = true;
        }
            if (collision.gameObject.CompareTag("Die") && !PerekrestokLevelController.IsGameEnded)
            {
                Counter.AddScore(1, GameType.CarGame);
                Debug.Log("Score: " + Counter.GetScore(GameType.CarGame));
                
                Destroy(gameObject);
            }
        }
    }
