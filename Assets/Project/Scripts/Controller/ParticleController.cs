using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public enum CharacterVFX
    {
        Move,
        Dash,
        Jump,
        Land,
    }
    [Serializable]
    public class CharParticle
    {
        public CharacterVFX name;
        public GameObject particle; // Giữ GameObject chung để chứa cả ParticleSystem và Trail
        public bool isTrailEffect = false; // Thêm flag để xác định là hiệu ứng trail hay không
    }

    public List<CharParticle> particles;

    public void ActivateParticleByName(CharacterVFX name)
    {
        foreach (var charParticle in particles)
        {
            if (charParticle.particle != null)
            {
                if (charParticle.name == name)
                {
                    if (charParticle.isTrailEffect)
                    {
                        charParticle.particle.SetActive(true); // Bật Trail
                    }
                    else
                    {
                        // Nếu không phải trail, giả định là particle burst/duration
                        if (charParticle.particle.TryGetComponent<ParticleSystem>(out var ps))
                        {
                            ps.Play(); // Phát ParticleSystem
                        }
                        else
                        {
                            Debug.LogWarning($"GameObject for VFX '{charParticle.name}' is not a ParticleSystem and not marked as Trail.  Using SetActive(true).");
                            charParticle.particle.SetActive(true); // Fallback nếu không có ParticleSystem và không phải Trail
                        }
                    }
                }
                else
                {
                    if (charParticle.isTrailEffect)
                    {
                        charParticle.particle.SetActive(false); // Tắt Trail khi không phải hiệu ứng hiện tại
                    }
                    else
                    {
                        // Tương tự, tắt ParticleSystem hoặc GameObject nếu không phải hiệu ứng hiện tại
                        if (charParticle.particle.TryGetComponent<ParticleSystem>(out var ps))
                        {
                            ps.Stop();
                        }
                        else
                        {
                            charParticle.particle.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Particle for VFX '{charParticle.name}' is not assigned in ParticleController on {gameObject.name}.");
            }
        }
    }


    // Hàm để tắt cụ thể một trail effect (nếu cần)
    public void DeactivateTrailByName(CharacterVFX trailName)
    {
        foreach (var charParticle in particles)
        {
            if (charParticle.particle != null && charParticle.name == trailName && charParticle.isTrailEffect)
            {
                charParticle.particle.SetActive(false);
                return; // Dừng sau khi tìm thấy và tắt trail cần thiết
            }
        }
    }

    // Hàm để tắt tất cả trail effects (ví dụ khi nhân vật nhảy lên)
    public void DeactivateAllTrails()
    {
        foreach (var charParticle in particles)
        {
            if (charParticle.particle != null && charParticle.isTrailEffect)
            {
                charParticle.particle.SetActive(false);
            }
        }
    }

    public void DeactiveExcept(CharacterVFX name)
    {
        foreach (var charParticle in particles)
        {
            if (charParticle.particle != null && charParticle.name != name)
            {
                charParticle.particle.SetActive(false);
            }
        }
    }
}