﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicadorSetor : MonoBehaviour
{
    public float tamanhoSetor;
    public SpriteRenderer fundoEstrelas, fundoPlaneta;
    Transform tr, cam_tr, estrelas_tr, planeta_tr;

    void Awake()
    {
        tr = GetComponent<Transform>();
        cam_tr = Camera.main.transform;
        estrelas_tr = fundoEstrelas.transform;
        planeta_tr = fundoPlaneta.transform;
    }

    void Update()
    {
        AtualizaTamanhoFundo(fundoEstrelas, tamanhoSetor + cam_tr.position.y * 0.06f);

        var estrelas_pos = estrelas_tr.position;
        estrelas_pos.y = cam_tr.position.y;
        estrelas_tr.position = estrelas_pos;

        var planeta_pos = planeta_tr.position;
        planeta_pos.y = (cam_tr.position.y*0.95f) + 12f;
        planeta_tr.position = planeta_pos;
    }

    void AtualizaTamanhoFundo(SpriteRenderer fundo, float tamanho)
    {
        fundo.size = new Vector2(30, tamanho);
    }

#if UNITY_EDITOR

    public float larguraPrev;
    public bool previsualizar;

    [ContextMenu("Atualizar setores")]
    void ContextoAtualizar() { Atualizar(); }
    void        OnValidate() { Atualizar(); }

    void Atualizar()
    {
        Transform setores_tr = transform.GetChild(0);

        for (int i = 0; i < setores_tr.childCount; i++)
        {
            var child = setores_tr.GetChild(i);
            var pos = child.localPosition;
            pos.y = tamanhoSetor * i;
            child.localPosition = pos;
        }
    }

    Vector3[] posicoes_setor = new Vector3[0];
    Vector3[] tamanhos_setor = new Vector3[0];

    void OnDrawGizmos()
    {
        if (!previsualizar)
            return;

        Transform setores_tr = transform.GetChild(0);
        var posInicial = setores_tr.position;

        var jogador_gbj = GameObject.FindWithTag("Player");
        if (jogador_gbj == null)
            return;

        var jogador_pos = jogador_gbj.transform.position;

        int qtd_setores = setores_tr.childCount;
        int i = 0;

        if (posicoes_setor.Length != qtd_setores)
        {
            posicoes_setor = new Vector3[qtd_setores];
            tamanhos_setor = new Vector3[qtd_setores];

            // calc posições
            for (i = 0; i < qtd_setores; i++)
            {
                posicoes_setor[i] = posInicial + new Vector3(0, i * tamanhoSetor, 0);
                tamanhos_setor[i] = new Vector3(larguraPrev, tamanhoSetor, 1f);
            }
        }
        else
        {
            for (i = 0; i < qtd_setores; i++)
            {
                Gizmos.color = i == 0 || setores_tr.GetChild(i).childCount > 0
                    ? Color.white
                    : Color.red;


                bool jogador_no_setor
                    =  jogador_pos.y < posicoes_setor[i].y + tamanhoSetor/2
                    && jogador_pos.y > posicoes_setor[i].y - tamanhoSetor/2;


                // calc tamanhos
                tamanhos_setor[i] = Vector3.Lerp(
                    tamanhos_setor[i],
                    new Vector3(
                        jogador_no_setor ? tamanhoSetor : larguraPrev,
                        tamanhoSetor,
                        1f
                    ),
                    Time.deltaTime * 4
                );

                // desenha setores
                Gizmos.DrawWireCube(
                    posicoes_setor[i],
                    tamanhos_setor[i]
                );

                if (jogador_no_setor)
                {
                    Gizmos.color = Color.gray;

                    // desenha setores
                    Gizmos.DrawWireCube(
                        posicoes_setor[i],
                        new Vector3(larguraPrev, tamanhoSetor, 1f)
                    );
                }
            }
        }
    }

#endif
}
