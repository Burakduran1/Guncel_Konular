package com.example.arkidsgame.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.TextView
import androidx.fragment.app.Fragment
import com.example.arkidsgame.CharacterCreationActivity
import com.example.arkidsgame.R

class EquipmentFragment : Fragment() {

    private lateinit var equipmentImageView: ImageView
    private lateinit var equipmentNameTextView: TextView
    private lateinit var equipmentDescriptionTextView: TextView
    private lateinit var equipmentButtons: List<ImageButton>
    
    // Seçilen ekipman
    private var currentEquipment = 0
    
    // Ekipman bilgileri
    private val equipmentNames = listOf("Sihirli Dürbün", "Pusula", "Not Defteri", "Fotoğraf Makinesi")
    private val equipmentDescriptions = listOf(
        "Uzaktaki nesneleri görmeni sağlar ve gizli detayları ortaya çıkarır!",
        "Her zaman doğru yönü gösterir ve kaybolmanı engeller!",
        "Keşiflerini not alman için özel bir defter!",
        "Gördüğün harika şeyleri fotoğraflamak için!"
    )
    private val equipmentImages = listOf(
        R.drawable.equipment_binoculars,
        R.drawable.equipment_compass,
        R.drawable.equipment_notebook,
        R.drawable.equipment_camera
    )
    
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_equipment, container, false)
        
        // UI elemanlarını tanımla
        equipmentImageView = view.findViewById(R.id.equipmentImageView)
        equipmentNameTextView = view.findViewById(R.id.equipmentNameTextView)
        equipmentDescriptionTextView = view.findViewById(R.id.equipmentDescriptionTextView)
        
        // Ekipman butonları
        equipmentButtons = listOf(
            view.findViewById(R.id.equipment1Button),
            view.findViewById(R.id.equipment2Button),
            view.findViewById(R.id.equipment3Button),
            view.findViewById(R.id.equipment4Button)
        )
        
        // Butonların görsellerini ayarla
        setupButtonImages()
        
        // Buton tıklama işlevlerini ayarla
        setupButtonListeners()
        
        // Varsayılan ekipmanı göster
        updateEquipmentDisplay()
        
        return view
    }
    
    private fun setupButtonImages() {
        // Ekipman butonları
        equipmentButtons.forEachIndexed { index, button ->
            button.setImageResource(equipmentImages[index])
        }
    }
    
    private fun setupButtonListeners() {
        equipmentButtons.forEachIndexed { index, button ->
            button.setOnClickListener {
                currentEquipment = index
                updateEquipmentDisplay()
                (activity as CharacterCreationActivity).updateEquipment(index)
            }
        }
    }
    
    private fun updateEquipmentDisplay() {
        // Seçilen ekipmanın bilgilerini göster
        equipmentNameTextView.text = equipmentNames[currentEquipment]
        equipmentDescriptionTextView.text = equipmentDescriptions[currentEquipment]
        equipmentImageView.setImageResource(equipmentImages[currentEquipment])
        
        // Seçili butonu vurgula
        equipmentButtons.forEachIndexed { index, button ->
            button.alpha = if (index == currentEquipment) 1.0f else 0.5f
        }
    }
} 