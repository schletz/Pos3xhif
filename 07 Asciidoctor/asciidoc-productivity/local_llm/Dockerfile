# docker build -t my-vllm-gemma4 .
FROM vllm/vllm-openai:cu130-nightly
RUN pip install --upgrade "transformers>=5.5.0"
RUN pip install nvidia-modelopt[torch]

