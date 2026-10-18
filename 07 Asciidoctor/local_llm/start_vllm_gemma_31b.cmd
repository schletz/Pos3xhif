docker build -t my-vllm .
docker create --gpus all ^
  -v %USERPROFILE%/.cache/huggingface:/root/.cache/huggingface ^
  -p 8000:8000 ^
  --name vllm-qwen3 ^
  my-vllm ^
  RedHatAI/Qwen3.6-35B-A3B-NVFP4 ^
  --reasoning-parser qwen3 ^
  --moe_backend flashinfer_cutlass ^
  --max-model-len 65536 ^
  --max-num-seqs 1 ^
  --max-num-batched-tokens 32768 ^
  --gpu-memory-utilization 0.9 ^
  --kv-cache-dtype fp8 ^
  --enable-prefix-caching ^
  --trust-remote-code ^
  --limit-mm-per-prompt "{\"image\": 0, \"video\": 0, \"audio\": 0}"