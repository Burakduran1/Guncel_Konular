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

class VehicleFragment : Fragment() {

    private lateinit var vehicleImageView: ImageView
    private lateinit var vehicleNameTextView: TextView
    private lateinit var vehicleDescriptionTextView: TextView
    private lateinit var vehicleButtons: List<ImageButton>
    
    // Seçilen araç
    private var currentVehicle = 0
    
    // Araç bilgileri
    private val vehicleNames = listOf("Sihirli Halı", "Mini Uçak", "Roket", "Sıcak Hava Balonu")
    private val vehicleDescriptions = listOf(
        "Gökyüzünde süzülerek keşif yapmanı sağlayan sihirli bir halı!",
        "Hızlı ve manevra kabiliyeti yüksek mini bir uçak!",
        "Uzaya kadar çıkabilen süper hızlı bir roket!",
        "Yavaş ama rahat bir şekilde keşif yapabileceğin renkli bir balon!"
    )
    private val vehicleImages = listOf(
        R.drawable.vehicle_magic_carpet,
        R.drawable.vehicle_airplane,
        R.drawable.vehicle_rocket,
        R.drawable.vehicle_balloon
    )
    
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_vehicle, container, false)
        
        // UI elemanlarını tanımla
        vehicleImageView = view.findViewById(R.id.vehicleImageView)
        vehicleNameTextView = view.findViewById(R.id.vehicleNameTextView)
        vehicleDescriptionTextView = view.findViewById(R.id.vehicleDescriptionTextView)
        
        // Araç butonları
        vehicleButtons = listOf(
            view.findViewById(R.id.vehicle1Button),
            view.findViewById(R.id.vehicle2Button),
            view.findViewById(R.id.vehicle3Button),
            view.findViewById(R.id.vehicle4Button)
        )
        
        // Butonların görsellerini ayarla
        setupButtonImages()
        
        // Buton tıklama işlevlerini ayarla
        setupButtonListeners()
        
        // Varsayılan aracı göster
        updateVehicleDisplay()
        
        return view
    }
    
    private fun setupButtonImages() {
        // Araç butonları
        vehicleButtons.forEachIndexed { index, button ->
            button.setImageResource(vehicleImages[index])
        }
    }
    
    private fun setupButtonListeners() {
        vehicleButtons.forEachIndexed { index, button ->
            button.setOnClickListener {
                currentVehicle = index
                updateVehicleDisplay()
                (activity as CharacterCreationActivity).updateVehicle(index)
            }
        }
    }
    
    private fun updateVehicleDisplay() {
        // Seçilen aracın bilgilerini göster
        vehicleNameTextView.text = vehicleNames[currentVehicle]
        vehicleDescriptionTextView.text = vehicleDescriptions[currentVehicle]
        vehicleImageView.setImageResource(vehicleImages[currentVehicle])
        
        // Seçili butonu vurgula
        vehicleButtons.forEachIndexed { index, button ->
            button.alpha = if (index == currentVehicle) 1.0f else 0.5f
        }
    }
} 