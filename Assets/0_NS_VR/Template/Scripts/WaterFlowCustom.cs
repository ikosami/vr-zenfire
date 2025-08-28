using UnityEngine;
using System.Collections.Generic;
using PaintIn3D;
using PaintCore;
public class WaterFlowCustom : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] ParticleSystem _particle;
    [SerializeField] CwPaintDecal[] _cwPaintDecals;

    List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    List<ParticleLine> particleLines = new List<ParticleLine>();

    struct ParticleLine {
        public List<Vector3> positions;
        public List<Vector3> velocities;
        public List<float> lifeTimes;

        public ParticleLine GetParticleLine(int index) {
            return new ParticleLine() {
                positions = new List<Vector3>(positions.GetRange(index, positions.Count - index)),
                velocities = new List<Vector3>(velocities.GetRange(index, velocities.Count - index)),
                lifeTimes = new List<float>(lifeTimes.GetRange(index, lifeTimes.Count - index)),
            };
        }

        public ParticleLine GetParticleLine(int index, int count) {
            return new ParticleLine() {
                positions = new List<Vector3>(positions.GetRange(index, count)),
                velocities = new List<Vector3>(velocities.GetRange(index, count)),
                lifeTimes = new List<float>(lifeTimes.GetRange(index, count)),
            };
        }
    }

    public bool IsGenerating = true;
    float firstSpeed = 5f;
    float updateSpan = 0.001f;
    float lifeTime = 0.6f;
    float _deltaTime = 0f;
    float gravity = 4.9f;
    float _arriveSoundTimer = 0f;

    void Awake() {
        _lineRenderers.Add(_lineRenderer);
        particleLines.Add(new ParticleLine() {
            positions = new List<Vector3>(),
            velocities = new List<Vector3>(),
            lifeTimes = new List<float>(),
        });

        int _lineRendererCount = 3;
        for (int i = 0; i < _lineRendererCount - 1; i++) {
            var lineRenderer = Instantiate(_lineRenderer, transform);
            _lineRenderers.Add(lineRenderer);
            particleLines.Add(new ParticleLine() {
                positions = new List<Vector3>(),
                velocities = new List<Vector3>(),
                lifeTimes = new List<float>(),
            });
        }
    }

    void UpdatePositions(float deltaTime) {
        for (int i = 0; i < particleLines.Count; i++) {
            var pL = particleLines[i];
            bool isPrevCollision = false;
            for (int j = 0; j < pL.positions.Count; j++) {

                pL.lifeTimes[j] -= deltaTime;
                if (pL.lifeTimes[j] <= 0) {
                    pL.positions.RemoveAt(j);
                    pL.velocities.RemoveAt(j);
                    pL.lifeTimes.RemoveAt(j);
                    j--;
                    continue;
                }

                // 当たり判定を取る
                var ray = new Ray(pL.positions[j], pL.velocities[j]);
                if (Physics.Raycast(ray, out var hit, pL.velocities[j].magnitude * deltaTime, _layerMask)) {
                    pL.positions.RemoveAt(j);
                    pL.velocities.RemoveAt(j);
                    pL.lifeTimes.RemoveAt(j);

                    _cwPaintDecals[Random.Range(0, _cwPaintDecals.Length)].HandleHitPoint(false, 0, 100, 0, hit.point, Quaternion.LookRotation(-hit.normal));
                    _particle.transform.position = hit.point;
                    _particle.Play();
                    _arriveSoundTimer -= deltaTime;
                    if (_arriveSoundTimer <= 0) {
                        SoundManager.Instance.Play("ketchup_arrive");
                        _arriveSoundTimer = 0.1f;
                    }
                    j--;
                    if(j != 0) isPrevCollision = true;
                    if(hit.collider.TryGetComponent(out OnDecal onDecal)) {
                        onDecal.TriggerEvent();
                    }
                    continue;
                }

                if (isPrevCollision) {
                    particleLines.Insert(i + 1, pL.GetParticleLine(j));
                    pL = pL.GetParticleLine(0, j);
                    particleLines[i] = pL;
                    break;
                }

                pL.positions[j] += pL.velocities[j] * deltaTime;
                pL.velocities[j] += Vector3.down * gravity * deltaTime;
            }
            if(pL.positions.Count == 0) {
                particleLines.RemoveAt(i);
                i--;
                continue;
            }
            // _lineRenderers[i].positionCount = pL.positions.Count;
            // _lineRenderers[i].SetPositions(pL.positions.ToArray());
        }
        
        // for(int i = particleLines.Count; i < _lineRenderers.Count; i++) {
        //     _lineRenderers[i].positionCount = 0;
        // }
    }

    void UpdateLineView() {
        var count = Mathf.Min(particleLines.Count, _lineRenderers.Count);
        for(int i = 0; i < count; i++) {
            _lineRenderers[i].positionCount = particleLines[i].positions.Count;
            _lineRenderers[i].SetPositions(particleLines[i].positions.ToArray());
        }
        for(int i = particleLines.Count; i < _lineRenderers.Count; i++) {
            _lineRenderers[i].positionCount = 0;
        }
    }

    void Update() {
        _deltaTime += Time.deltaTime;
        if (_deltaTime >= updateSpan) {
            UpdatePositions(_deltaTime);
            if (IsGenerating) {
                AddParticle(transform.position, transform.forward * firstSpeed, lifeTime);
            }
            UpdateLineView();
            _deltaTime = 0;
        }
    }

    public void AddParticle(Vector3 position, Vector3 velocity, float lifeTime) {
        if(particleLines.Count == 0) {
            particleLines.Add(new ParticleLine() {
                positions = new List<Vector3>() { position },
                velocities = new List<Vector3>() { velocity },
                lifeTimes = new List<float>() { lifeTime },
            });
        } else {
            var pL = particleLines[0];
            pL.positions.Insert(0, position);
            pL.velocities.Insert(0, velocity);
            pL.lifeTimes.Insert(0, lifeTime);
            particleLines[0] = pL;
        }
    }

    public void StartGenerating() {
        IsGenerating = true;
    }

    public void StopGenerating() {
        IsGenerating = false;
    }
}
