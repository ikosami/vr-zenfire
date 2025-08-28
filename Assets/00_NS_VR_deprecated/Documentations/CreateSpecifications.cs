using UnityEngine;

namespace NS.Documents
{
    public class CreateSpecifications : MonoBehaviour
    {
        void GenerateSpecifications()
        {
            // 仕様の生成処理
            // 実際のプロジェクトでは必要に応じて実装
        }

        string GetEventSystemSpec()
        {
            return @"
                // Event System Specification
                sig Event {
                    name: String,
                    data: IEvent
                }
                
                sig EventManager {
                    events: Event -> Action,
                    instance: lone EventManager
                }
                
                fact EventManagerSingleton {
                    one EventManager
                    all em: EventManager | em.instance = em
                }
            ";
        }

        string GetReferencesSpec()
        {
            return @"
                // References System Specification
                sig References {
                    instance: lone References,
                    player: Transform,
                    prefabs: String -> GameObject,
                    decals: String -> CwPaintDecal,
                    particles: String -> ParticleSystem
                }
                
                fact ReferencesSingleton {
                    one References
                    all r: References | r.instance = r
                }
            ";
        }
    }
} 