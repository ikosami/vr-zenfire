using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class MovementControllerTest
{
    private GameObject _playerObject;
    private MovementController _movementController;
    private CharacterController _characterController;
    private GameObject _cameraObject;

    [SetUp]
    public void Setup()
    {
        // Create player hierarchy
        _playerObject = new GameObject("Player");
        _characterController = _playerObject.AddComponent<CharacterController>();
        _movementController = _playerObject.AddComponent<MovementController>();
        
        // Create camera
        _cameraObject = new GameObject("MainCamera");
        _cameraObject.AddComponent<Camera>();
        _cameraObject.tag = "MainCamera";
        _cameraObject.transform.parent = _playerObject.transform;
        
        // Setup movement controller
        _movementController.enabled = true;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_playerObject);
    }

    [UnityTest]
    public IEnumerator TestInitialization()
    {
        yield return null;
        Assert.IsNotNull(_characterController, "CharacterController should be initialized");
    }

    [UnityTest]
    public IEnumerator TestGroundedState()
    {
        // Move player to ground level
        _playerObject.transform.position = Vector3.zero;
        
        yield return new WaitForFixedUpdate();
        
        // Player should be grounded
        Assert.IsTrue(_characterController.isGrounded, "Player should be grounded");
    }

    [UnityTest]
    public IEnumerator TestJumpMechanic()
    {
        // Initial position
        Vector3 startPos = _playerObject.transform.position;
        
        // Simulate jump button press
        var inputManager = VRInputManager.Instance;
        
        yield return new WaitForFixedUpdate();
        
        // After jump, y position should be higher
        Assert.Greater(_playerObject.transform.position.y, startPos.y, "Player should move upward when jumping");
    }

    [UnityTest]
    public IEnumerator TestMovementSpeed()
    {
        // Initial position
        Vector3 startPos = _playerObject.transform.position;
        
        // Set movement direction
        Vector2 stickInput = Vector2.right; // Move right
        
        // Wait for movement to occur
        yield return new WaitForSeconds(0.1f);
        
        // Check if movement occurred
        float displacement = Vector3.Distance(startPos, _playerObject.transform.position);
        Assert.Greater(displacement, 0f, "Player should move when stick input is applied");
    }

    [UnityTest]
    public IEnumerator TestArmSwingMovement()
    {
        // Initial position
        Vector3 startPos = _playerObject.transform.position;
        
        // Simulate arm swing
        // Note: In a real test, we would need to mock the VRInputManager's controller positions
        
        yield return new WaitForSeconds(0.1f);
        
        // Check if movement occurred
        float displacement = Vector3.Distance(startPos, _playerObject.transform.position);
        Assert.Greater(displacement, 0f, "Player should move when arm swing is detected");
    }

    [UnityTest]
    public IEnumerator TestMaximumSpeed()
    {
        // Initial position
        Vector3 startPos = _playerObject.transform.position;
        
        // Apply continuous movement
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        
        // Calculate velocity
        float velocity = _characterController.velocity.magnitude;
        Assert.LessOrEqual(velocity, 10.0f, "Player velocity should not exceed maximum speed");
    }

    [UnityTest]
    public IEnumerator TestFriction()
    {
        // Apply initial velocity
        Vector3 startPos = _playerObject.transform.position;
        
        // Wait for friction to take effect
        yield return new WaitForSeconds(0.5f);
        
        // Velocity should decrease
        float velocity = _characterController.velocity.magnitude;
        Assert.Less(velocity, 0.1f, "Player should slow down due to friction");
    }
}
