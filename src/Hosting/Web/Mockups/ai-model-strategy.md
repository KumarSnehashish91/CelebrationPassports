# Decision Note: AI Model Strategy (Ollama) — Insight Generation & Trip Planning

**Status:** Decided — staged approach, not a one-time build
**Owner component:** `AI Services` (as defined in `Celebration_Passport_System_Design_v1.docx`)
**Related specs:** `gift-story-print-preview.md` (insight generation consumer), existing itinerary generation feature (trip planning consumer)

---

## 1. The Decision

We are **not** training a model from scratch (cost/complexity is entirely out of proportion to a solo-founder project) and we are **not** fine-tuning a model right now. We considered it and are deliberately deferring it.

Instead, AI Services is built in stages, in this order:

1. **Now:** Self-hosted open models via Ollama, used as-is, improved through prompt engineering only.
2. **Now, trip planning only:** Add a RAG (retrieval-augmented generation) layer — real, verified destination data fed to the model at generation time, rather than relying on the model's own knowledge.
3. **After real usage exists:** Start capturing the gap between what the model generates and what users actually keep/edit, with explicit user consent.
4. **Only once (3) produces a specific, provable quality problem prompting can't fix:** Consider LoRA/QLoRA fine-tuning on top of whichever open model is current at that point.

**Why this order, not fine-tuning first:** fine-tuning requires a training dataset of real, high-quality examples in the desired style. We have zero real users and zero real content right now — there is nothing to fine-tune on yet. Building a fine-tuned model before real usage means guessing at what "good" looks like, not learning it.

---

## 2. What This Means for Each Feature Right Now

### Insight Generation (from `gift-story-print-preview.md`, Section 7, open question 1)
- **Requires a vision-capable model**, not a text-only one — this is a hard requirement, not a nice-to-have. Confirm the Ollama deployment includes a vision-capable model (e.g., LLaVA, Qwen2-VL, or Llama 3.2 Vision — pick based on what's realistic to self-host at the expected concurrency).
- Do **not** attempt fine-tuning for this feature yet. Invest instead in:
  - A well-constructed system prompt defining tone (warm, specific, non-generic — avoid "This is a lovely photo" style output)
  - Few-shot examples embedded in the prompt (2-4 strong example photo-descriptions → insight pairs) to anchor style without training
  - If output quality is still weak after prompt iteration, that's a signal to revisit model choice before considering fine-tuning

### Trip Planning / Itinerary Generation (existing feature, text-only)
- Add a **RAG layer**: a curated, verified database of destination information (opening hours, real place names, realistic travel times between locations) that gets retrieved and injected into the prompt at generation time.
- This is a **data/infrastructure task, not a training task** — the model's job is to write good itinerary prose around facts it's given, not to recall facts from its own weights. This also solves the classic LLM failure mode of confidently inventing plausible-but-wrong details (a closed restaurant, an inaccurate travel time), which matters more here than in insight generation since a user might actually follow a bad itinerary.
- Do not attempt fine-tuning for this feature yet, for the same data-availability reason as above.

---

## 3. Data Collection for Future Fine-Tuning (Stage 3 — not active yet)

When this stage is activated (post-launch, with real usage):
- Requires explicit, opt-in user consent — do not collect generation-vs-edit data by default. This needs to be reflected in the Privacy Policy before it's turned on, consistent with the product's Privacy First value.
- Track: original AI-generated text (insight or itinerary) alongside the final user-edited/kept version, when a user makes an edit. The delta between these is the actual training signal — silence (user keeps the AI output unedited) is a weaker signal and shouldn't be over-weighted as "this was good."
- Do not build the collection pipeline speculatively before Stage 2 is live and Stage 1 output has been observed in real use — there is no useful data to collect before then.

---

## 4. Explicit Non-Goals (for now)

- No training a model from scratch.
- No fine-tuning before real usage data exists.
- No collecting training data without explicit user consent.
- No blocking v1.0 launch on any of this — Stage 1 (base model + good prompting) is sufficient to ship.
