docker build -t my-vllm-gemma4 .
docker create --gpus all ^
  -v %USERPROFILE%/.cache/huggingface:/root/.cache/huggingface ^
  -p 8000:8000 ^
  --name vllm-gemma4 ^
  my-vllm-gemma4 ^
  --model LilaRest/gemma-4-31B-it-NVFP4-turbo ^
  --quantization modelopt ^
  --max-model-len 16384 ^
  --max-num-seqs 128 ^
  --max-num-batched-tokens 16384 ^
  --gpu-memory-utilization 0.9 ^
  --kv-cache-dtype fp8 ^
  --enable-prefix-caching ^
  --trust-remote-code
