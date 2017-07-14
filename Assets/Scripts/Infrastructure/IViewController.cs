using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IViewController
{
    void Initialize(Transform parent);

    void SetActive(bool state);
}
